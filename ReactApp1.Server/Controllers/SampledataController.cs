using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReactApp1.Server.Models;
using System.Data;
using System.Runtime.InteropServices.ComTypes;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using TimescaleSample.Models;
using WebApp2.Model;

namespace ReactApp1.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        DatabaseContext DB = new DatabaseContext();
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }


        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<Tick_Data> Get()
        {
            var sql = """
                SELECT * FROM "ticks" order by id limit 100  OFFSET (SELECT COUNT(*) FROM ticks) - 100
                """;
            int cnt = DB.Ticks.FromSqlRaw(sql).Count();
            Console.WriteLine("->>>" + cnt);
            //Enumerable data=new Enumerable()
            return DB.Ticks.FromSqlRaw(sql).ToList().Select(tick => new Tick_Data
            {
                id=tick.Id,
                time = tick.Time ,
                symbol = tick.Symbol + "",
                price = tick.Price,
                day_volume = tick.Day_volume + " "
            });

        }
        
        [HttpPost("updateRow")]
        public IActionResult update([FromBody] Tick_Data tickData)
        {
            
            if (tickData == null)
            {
                return BadRequest("Invalid data");
            }
            Console.WriteLine("update"+tickData.id);
            if (tickData.id == -1) {
                
                int newID = DB.Ticks.Max(p => p.Id) + 1;
                Console.WriteLine(newID);
                int cnt = DB.Database.ExecuteSqlRaw("insert into ticks values({0}, {1}, {2}, {3}, {4})", newID, tickData.time, tickData.symbol, tickData.price, tickData.day_volume);
                return Ok("newId:" + newID);
            }
            else
            {
                var existingTick = DB.Ticks.Single(t => t.Id == tickData.id);

                if (existingTick == null)
                {
                    return NotFound("Tick data not found");
                }

                // Update properties of the existing entity
                existingTick.Time = tickData.time;
                existingTick.Symbol = tickData.symbol;
                existingTick.Price = tickData.price;
                existingTick.Day_volume = tickData.day_volume;
                // Save changes to the database
                DB.SaveChanges();
            }
            return Ok("Data saved successfully");
        }
        [HttpPost("deleteRows")]
        public IActionResult delete([FromBody] String_data data)
        {
            Console.WriteLine("dd->>>" + data.stringData);
            if (data.stringData == null)
            {
                return BadRequest("Invalid data");
            }
            //FormattableString str = $"delete from ticks where id in ({data.stringData})";
            
            int r = DB.Database.ExecuteSqlRaw("delete from ticks where id in (" + data.stringData + ")");
            
            //DB.SaveChanges();
            return Ok("Data deleted successfully");
        }
        [HttpPost("LogData")]
        public IActionResult PostLogData([FromBody] object  receive)
        {
            try
            {
                //Console.WriteLine("body:" + receive);
                receiveData rec = JsonSerializer.Deserialize<receiveData>(receive + "");
                string data = rec.data;
                //{ "serial": "324affe"}
                //20240117 14:11:41 gui INFO 4 6; None; None 20240117 14:11:41 gui INFO 14 5; treehandler; None 20240117 14:11:41 gui INFO 4 20; .. / nonvol / gui /; gui_dir 20240117 14:11:41 gui INFO 0 5; ('192.168.150.43', 1883); pc - gui
                //data = "{\"serial\": \"F812\"}\r\n1710166717 ebus INFO ebs;start;None;None\r\n1710166717 ebus INFO mqt;init;('localhost', 1883);ebus\r\n1710166717 ebus INFO dtt;init;('ebus', False, 'ebus.yaml');treehandler_mqtt\r\n1710166717 ebus INFO dtt;init;treehandler;None\r\n1710166717 config INFO cfg;start;None;None\r\n1710166717 config INFO dtt;init;treehandler;None\r\n1710166717 config INFO cfg;git;dir_exists;init_repo\r\n1710166717 ebus INFO ebs;load_hex;get_hex_file;ep-rog-2022.hex\r\n1710166718 ebus INFO mqt;start_loop;None;None\r\n1710166718 ebus INFO mqt;connected;None;None\r\n1710166719 config INFO cfg;load;yaml;loaded_yaml\r\n1710166719 config INFO mqt;init;('localhost', 1883);config\r\n1710166719 config INFO dtt;init;('config', False, None);treehandler_mqtt\r\n1710166719 config INFO cfg;load;json;file_load\r\n1710166720 ebus ERROR dtt;request_item;config;timeout\r\n1710166722 ebus ERROR dtt;request_item;system;timeout\r\n1710166723 config INFO mqt;start_loop;None;None\r\n1710166723 config INFO mqt;connected;None;None\r\n1710166724 aio INFO und;start;None;None\r\n1710166724 aio INFO dio;init;init;Dummy Mode: False\r\n";
                String[] logLines = Regex.Split(data, "\r\n|\r|\n");
                jsonData json = JsonSerializer.Deserialize<jsonData>(logLines[0]);
                Console.WriteLine("cc:" + logLines.Length);
                Console.WriteLine("serival:" + json.serial);
                string serial = json.serial;
                if (serial.Length > 4) serial = serial.Substring(0, 4);

                for (int i = 1; i < logLines.Length; i++)
                {
                    string line = logLines[i];
                    if (line != "")
                    {
                        string[] values = line.Split(new char[] { ' ', ';' });
                        string created = localtime(long.Parse(values[0]));

                        string component = values[1];
                        string ev = values[2];
                        string short_1 = values[3];
                        string short_2 = values[4];
                        string information = values[5];
                        if (component.Length > 3) component = component.Substring(0, 3);
                        if (ev.Length > 3) ev = ev.Substring(0, 4);
                        if (short_1.Length > 10) short_1 = short_1.Substring(0, 10);
                        if (short_2.Length > 10) short_2 = short_2.Substring(0, 10);
                        var logdata = new LogData()
                        {
                            Serial = serial,
                            Created = DateTime.Parse(created).ToUniversalTime(),
                            Component = component,
                            Event = ev,
                            Short_1 = short_1,
                            Short_2 = short_2,
                            Information = information
                        };
                        DB.LogDatas.Add(logdata);
                        //string sql = string.Format("insert into log_data_ (serial, created, component, event, short_1, short_2, information) values('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}')", json.serial, created, component, ev, short_1, short_2, information);// $"insert into log_data_ (serial, created, component, event, short_1, short_2, information) values('{serial}', '17:20:35', '{component}', '{ev}', '{short_1}', '{short_2}', '{information}')";
                        //Console.WriteLine(sql);
                        //int cnt = DB.Database.ExecuteSqlRaw(sql);
                    }
                    DB.SaveChanges();
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        public static string localtime(long unixTime)
        {
            // Convert Unix time to DateTime
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(unixTime);
            string time = dateTimeOffset.LocalDateTime.ToString("HH:mm:ss");

            // Display the result
            Console.WriteLine("Unix time: " + unixTime);
            Console.WriteLine("DateTime: " + dateTimeOffset.LocalDateTime);
            return time;
        }

    }
    public class jsonData
    {
        public string serial { get; set; }
    }
    public class receiveData
    {
        public string data { get; set; }
    }
}
