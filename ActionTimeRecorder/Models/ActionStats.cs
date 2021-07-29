using Newtonsoft.Json;

namespace ActionTimeRecorder.Models
{
    public class ActionStats
    {
        [JsonProperty("action")]
        public string Action { get; set; }

        [JsonProperty("avg")]
        public double Avg { get; set; }
    }
}
