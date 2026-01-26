using System.Net.Http.Json;
using WebApplication1;

namespace Interfata
{
    public static class ApiRequests
    {
        private static readonly string url = "http://localhost:5101/api/node";
        public static async Task<List<NodeSummary>> GetNodes()
        {
            using HttpClient client = new HttpClient();
            try
            {
                string final_url = $"{url}/nodes";

                var nodes = await client.GetFromJsonAsync<List<NodeSummary>>(final_url);

                return nodes ?? new List<NodeSummary>();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Connection error: {ex.Message}");
                return new List<NodeSummary>();
            }
        }

        public static async Task<List<string>> GetNodeTypes(string nodeName)
        {
            using HttpClient client = new HttpClient();
            try
            {                
                string final_url = $"{url}/nodes/{nodeName}/types";

                var nodeTypes = await client.GetFromJsonAsync<List<string>>(final_url);

                return nodeTypes ?? new List<string>();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error obtaining sensors for {nodeName}: {ex.Message}");
                return new List<string>();
            }
        }

        public static async Task<NodeValues?> GetNodeTypeValueLatest(string nodeName , string type)
        {
            using HttpClient client = new HttpClient();
            try
            {
                string final_url = $"{url}/nodes/{nodeName}/{type}/latest";

                var value = await client.GetFromJsonAsync<NodeValues>(final_url);

                return value;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error obtaining node from {nodeName} {type} latest value: {ex.Message}");
                return null;
            }
        }

    }
}   
