namespace WebApplication1
{
    public class Node
    {
        public string NodeName { get; set; } = string.Empty;

        public List<SensorReading> Sensors { get; set; } = new List<SensorReading>();

    }

    public class NodeSummary
    {   
        public int NodeId { get; set; }
        public string NodeName { get; set; } = string.Empty;
       
    }

    public class NodeValues
    {
        public string NodeName { get; set; } = string.Empty;
        public string SensorType { get; set; } = string.Empty;
        public float SensorValue { get; set; }

        public DateTime RecordedAt { get; set; }
    }
}
