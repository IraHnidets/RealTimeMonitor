using Microsoft.AspNetCore.SignalR;
using RealTimeCommunication.Hubs;
using RealTimeCommunication.Models;

namespace RealTimeCommunication.Services
{
    public class SensorBroadcastService : BackgroundService
    {
        private readonly IHubContext<SensorHub> _hubContext;
        private readonly Random _random = new();
        private double _temp = 22.0;
        private double _humidity = 55.0;
        private double _pressure = 1013.0;

        public static List<SensorData> History { get; } = new();
        public static List<Alert> Alerts { get; } = new();
        public static bool IsRunning { get; set; } = true;

        public SensorBroadcastService(IHubContext<SensorHub> hubContext)
        {
            _hubContext = hubContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (IsRunning)
                {
                    _temp += (_random.NextDouble() - 0.5) * 0.8;
                    _temp = Math.Clamp(_temp, 15, 40);

                    _humidity += (_random.NextDouble() - 0.5) * 1.2;
                    _humidity = Math.Clamp(_humidity, 30, 90);

                    _pressure += (_random.NextDouble() - 0.5) * 0.5;
                    _pressure = Math.Clamp(_pressure, 1005, 1025);

                    var data = new SensorData
                    {
                        Timestamp = DateTime.Now,
                        Temperature = Math.Round(_temp, 1),
                        Humidity = Math.Round(_humidity, 1),
                        Pressure = Math.Round(_pressure, 1)
                    };

                    History.Add(data);
                    if (History.Count > 100) History.RemoveAt(0);

                    CheckAlerts(data);

                    await _hubContext.Clients.All.SendAsync("ReceiveSensorData", data, stoppingToken);
                }

                await Task.Delay(1000, stoppingToken);
            }
        }

        private void CheckAlerts(SensorData data)
        {
            if (data.Temperature > 35)
                Alerts.Insert(0, new Alert { Sensor = "Температура", Value = data.Temperature, Threshold = 35, Type = "danger", Timestamp = data.Timestamp });
            if (data.Humidity > 80)
                Alerts.Insert(0, new Alert { Sensor = "Вологість", Value = data.Humidity, Threshold = 80, Type = "warning", Timestamp = data.Timestamp });
            if (Alerts.Count > 20) Alerts.RemoveRange(20, Alerts.Count - 20);
        }
    }
}