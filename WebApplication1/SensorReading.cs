namespace WebApplication1
{
    public class SensorReading
    {
        public string SensorType { get; set; } = string.Empty;
        public float SensorValue { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
