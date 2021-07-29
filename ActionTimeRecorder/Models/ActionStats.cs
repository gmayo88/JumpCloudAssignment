using Newtonsoft.Json;

namespace ActionTimeRecorder.Models
{
    class ActionStats
    {
        [JsonProperty("action")]
        public string Action { get; set; }

        [JsonProperty("avg")]
        public double Avg { get; set; }
    }
}
