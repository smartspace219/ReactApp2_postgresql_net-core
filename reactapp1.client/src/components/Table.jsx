import { useState, useMemo, useEffect } from 'react';
import { useTable } from 'react-table';
import axios from "axios";
const CustomInputComponent = ({ value, onChange }) => {
    const [inputValue, setInputValue] = useState(value);

    const handleInputChange = (e) => {
        setInputValue(e.target.value);
        onChange(e.target.value);
    };

    return (
        <input
            value={inputValue}
            onChange={handleInputChange}
        // Additional props and styling
        />
    );
};

export default function App() {

    const initialData = useMemo(
        () => [
            { checked: 0, id: -1, time: '1', symbol: 'Alice', price: 'Johnson', day_volume: 'asd' },
            { checked: 0, id: -1, time: '2', symbol: 'asd', price: '43542', day_volume: 'sad' },
            // Add more row data here
        ],
        []
    );
    useEffect(() => {
        //populateWeatherData();
    }, []);
    const columns = useMemo(
        () => [
            { Header: 'Check', accessor: 'checked' },
            { Header: 'Time', accessor: 'time' },
            { Header: 'Symbol', accessor: 'symbol' },
            { Header: 'Price', accessor: 'price' },
            { Header: 'Day volume', accessor: 'day_volume' }
        ],
        []
    );

    const [editRowId, setEditRowId] = useState(null);
    const [tableData, setData] = useState(initialData);
    const {
        getTableProps,
        getTableBodyProps,
        headerGroups,
        rows,
        prepareRow,
    } = useTable({ columns, data: tableData });
    const handleEditClick = (rowId) => {
        setEditRowId(rowId);
    };

    const handleInputChange = (rowIndex, columnId, newValue) => {
        const newData = [...tableData];
        newData[rowIndex][columnId] = newValue;
        let newRowId = -1;
        if (columnId != "checked") {

            let time = newData[rowIndex]["time"];
            let price = newData[rowIndex]["price"];
            let symbol = newData[rowIndex]["symbol"];
            let day_volume = newData[rowIndex]["day_volume"];
            let rowdata = newData[rowIndex];

            saveRowData(rowdata, rowIndex);
        }
        setData(newData);
    };
    const rowAdd = () => {
        setData((data) => [...data, { checked: 0, id: -1, time: '', symbol: '', price: '200.00', day_volume: '' }]);
    };
    return (
        
        <div className='mainDiv'>
            <textarea id="log_data" style={{ width: "500px", height:"200px" }}  />
            <button onClick={() => logdataSend()}>send</button>
            
        </div>
    );
    async function logdataSend() {
        let logdata = document.getElementById("log_data").value;
        
        try {
            const response = await axios.post('weatherforecast/Logdata', { "data": logdata });
            alert();
            console.log('Response:', response.data);
            return response.data;
        } catch (error) {
            
            console.error('Error:', error.message);
            throw error;
        }
        
    }
    async function populateWeatherData() {
        try {
            const response = await axios.get("weatherforecast");
            setData(response.data);
        } catch (error) {
            console.error("Error fetching data:", error);
        }
    }
    async function deleteRowData() {

        const newData = [...tableData];
        let selectedData = newData.filter(data => data.checked == 1);
        let rowsStr = "";
        if (selectedData.length > 0) {
            selectedData.forEach(element => {
                rowsStr += element["id"] + ",";
            });
            rowsStr = rowsStr.substr(0, rowsStr.length - 1);
        }

        try {
            const response = await axios.post("weatherforecast/deleteRows", {
                "stringData": rowsStr
            });
            let currData = newData.filter(data => data.checked != 1);
            setData(currData);
        } catch (error) {
            console.error("Error deleting data:", error);
        }

    }
    async function saveRowData(row, rowIndex) {
        try {

            const response = await axios.post("weatherforecast/updateRow", {
                "id": row["id"],
                "time": row["time"],
                "symbol": row["symbol"],
                "price": row["price"],
                "day_volume": row["day_volume"]
            });
            if (response.data.indexOf("newId") == 0) {

                let newId = response.data.split(":")[1] * 1;
                const newData = [...tableData];
                newData[rowIndex]["id"] = newId;
                setData(newData);
            }

            //setData(response.data);
        } catch (error) {
            console.error("Error fetching data:", error);
        }

    }
}