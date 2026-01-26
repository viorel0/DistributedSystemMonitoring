using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Dapper;


namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // base URL: /api/node
    public class NodeController : ControllerBase
    {
        private readonly string _connectionString;

        public NodeController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("No connection string set in json");
        }

        [HttpPost("upload")]
        public async Task<IActionResult> PostData([FromBody] Node data)
        {
            if (data == null || string.IsNullOrEmpty(data.NodeName))
            {
                return BadRequest("Invalid Data.");
            }
            using var connection = new SqlConnection(_connectionString);

            string nodeSql = @"
            IF NOT EXISTS (SELECT 1 FROM Nodes WHERE NodeName = @name)
            BEGIN
                INSERT INTO Nodes (NodeName) VALUES (@name);
            END
            SELECT NodeID FROM Nodes WHERE NodeName = @name;";

            int nodeId = await connection.QuerySingleAsync<int>(nodeSql, new { name = data.NodeName });

            foreach (var sensor in data.Sensors)
            {
                string measureSql = @"
            INSERT INTO Measurements (NodeID, SensorType, SensorValue)
            VALUES (@nodeId, @type, @val);";

                await connection.ExecuteAsync(measureSql, new
                {
                    nodeId = nodeId,
                    type = sensor.SensorType,
                    val = sensor.SensorValue
                });
            }

            return Ok(new { message = $"Succes! Am salvat {data.Sensors.Count} senzori pentru {data.NodeName}" });
        }

        [HttpGet("nodes")]
        public async Task<IActionResult> GetNodes()
        {
            using var connection = new SqlConnection(_connectionString);
            string sql = "SELECT NodeID,NodeName FROM Nodes;";
            var nodes = await connection.QueryAsync<NodeSummary>(sql);
            return Ok(nodes);
        }

        [HttpGet("nodes/{nodename}/types")]
        public async Task<IActionResult> GetNodeTypesByName(string nodename)
        {
            using var connection = new SqlConnection(_connectionString);

            string sql = @"
            SELECT DISTINCT M.SensorType 
            FROM Measurements M
            INNER JOIN Nodes N ON M.NodeID = N.NodeID
            WHERE N.NodeName = @name";

            var types = await connection.QueryAsync<string>(sql, new { name = nodename });

            if (types == null || !types.Any())
            {
                return NotFound("Nu am găsit senzori pentru acest nod.");
            }

            return Ok(types);
        }

        [HttpGet("nodes/{nodename}/{type}/latest")]
        public async Task<IActionResult> GetLatestNodeValuesByName(string nodename, string type)
        {
            using var connection = new SqlConnection(_connectionString);

            string sql = @"
            SELECT TOP 1 N.NodeName, M.SensorType, M.SensorValue, M.RecordedAt
            FROM Measurements M
            INNER JOIN Nodes N ON M.NodeID = N.NodeID
            WHERE N.NodeName = @nodename
            AND M.SensorType = @type
            ORDER BY M.RecordedAt DESC;";

            var value = await connection.QueryFirstOrDefaultAsync<NodeValues>(sql, new { nodename , type });

            if (value == null)
            {
                return NotFound("Nu am găsit nicio valoare recentă.");
            }

            return Ok(value);
        }

    }
}
