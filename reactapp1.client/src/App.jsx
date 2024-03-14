import { useEffect, useState } from 'react';
import './App.css';

function App() {
    const [forecasts, setForecasts] = useState([]);

    useEffect(() => {
        populateWeatherData();
    }, []);
    const handleDelete = (rowId) => {
        const list = [...forecasts];
        list.splice(rowId, 1);

        setForecasts(list);
    };
    const contents = forecasts === undefined
        ? <p><em>Loading... Please refresh once the ASP.NET backend has started. See <a href="https://aka.ms/jspsintegrationreact">https://aka.ms/jspsintegrationreact</a> for more details.</em></p>
        : <table className="table table-striped" aria-labelledby="tabelLabel">
            <thead>

                <tr>
                    <th>no</th>
                    <th>time</th>
                    <th>symbol</th>
                    <th>price</th>
                    <th>day_volume</th>
                </tr>
            </thead>
            <tbody>
                {
                    forecasts.map((forecast, i) => {

                        return (
                            <tr key={i}>
                                <td><button onClick={() => handleDelete(i)}>delete</button></td>
                                <td><input value={forecast.time} onChange={() => rowChange()} /></td>
                                <td><input value={forecast.symbol} onChange={() => rowChange()} /></td>
                                <td><input value={forecast.price} onChange={() => rowChange()} /></td>
                                <td><input value={forecast.day_volume} conchange={() => rowChange()} /></td>
                            </tr>)
                    }
                    )}
            </tbody>
        </table>;

    return (
        <div><button onClick={() => populateWeatherData()}>reload</button>
            <h1 id="tabelLabel">Weather forecast</h1>
            <p>This component demonstrates fetching data from the server.</p>
            <di><button onClick={() => pushData()}>add</button>
                <button onClick={() => pushData()}>update</button>
            </di>
            {contents}
        </div>
    );
    async function pushData() {
        const response = await fetch('weatherforecast');
        const data = await response.json();
        setForecasts(data);
    }
    async function populateWeatherData() {
        const response = await fetch('weatherforecast');
        const data = await response.json();
        setForecasts(data);
    }
}

export default App;