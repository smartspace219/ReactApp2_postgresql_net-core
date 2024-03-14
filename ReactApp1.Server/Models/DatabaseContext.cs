using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
namespace WebApp2.Model
{
    public class DatabaseContext : DbContext
    {
        public DbSet<TickData> Ticks { get; set; }
        public DbSet<LogData> LogDatas { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(CONNECTION_STRING);
            base.OnConfiguring(optionsBuilder);
        }
        private const string CONNECTION_STRING = "Host=localhost;Port=5432;" +
                    "Username=postgres;" +
                    "Password=123;" +
                    "Database=sample";
        //private const string CONNECTION_STRING = "Host=gdzucz4ndf.gj9oid3jj5.tsdb.cloud.timescale.com;Port=32436;" +
        //            "Username=tsdbadmin;" +
        //            "Password=mywebworld0219;" +
        //            "Database=tsdb";//database name
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<TickData>(e => e.ToTable("ticks"));
            //modelBuilder.Entity<LogData>(e => e.ToTable("log_data"));
            base.OnModelCreating(modelBuilder);
        }
        
        public async Task<TickData> Get(int id)
        {
            using (var db = new DatabaseContext())
            {
                return await db.Ticks.FindAsync(id);
            }
        }

        public async Task<IEnumerable<TickData>> GetAll()
        {
            using (var db = new DatabaseContext())
            {
                return await db.Ticks.ToListAsync();
            }
        }
        public async Task Update(int id, TickData tdata)
        {
            using (var db = new DatabaseContext())
            {
                db.Ticks.Update(tdata);
                await db.SaveChangesAsync();
            }
        }
        public async Task Delete(int id)
        {
            using (var db = new DatabaseContext())
            {
                var game = await db.Ticks.FindAsync(id);
                if (game == null)
                    return;

                db.Ticks.Remove(game);
                await db.SaveChangesAsync();
            }
        }
    }
    [Table("log_data")]
    public class LogData
    {
        [System.ComponentModel.DataAnnotations.Key]

        [Column("id")]
        public int Id { get; set; }
        
        [Column("serial")]
        public string Serial { get; set; }

        [Column("created")]
        public DateTime Created { get; set; }

        [Column("component")]
        public string Component { get; set; }

        [Column("event")]
        public string Event  { get; set; }
        
        [Column("short_1")]
        public string Short_1 { get; set; }

        [Column("short_2")]
        public string Short_2 { get; set; }
        [Column("information")]
        public string Information { get; set; }
    }
    [Table("ticktable")]
    public class TickData
    {
        [System.ComponentModel.DataAnnotations.Key]

        [Column("id")]
        public int Id { get; set; }

        [Column("time")]
        public string Time { get; set; }

        [Column("symbol")]
        public string Symbol { get; set; }

        [Column("price")]
        public double Price { get; set; }

        [Column("day_volume")]
        public string Day_volume { get; set; }
    }
}
