namespace RealTimeCommunication.Models
{
    public class SensorData
    {
        public DateTime Timestamp { get; set; }
        public double Temperature { get; set; }
        public double Humidity { get; set; }
        public double Pressure { get; set; }
    }

    public class Alert
    {
        public string Sensor { get; set; } = "";
        public double Value { get; set; }
        public double Threshold { get; set; }
        public string Type { get; set; } = "";
        public DateTime Timestamp { get; set; }
    }
}