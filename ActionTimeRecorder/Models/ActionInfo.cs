using Newtonsoft.Json;

namespace ActionTimeRecorder.Models
{
    class ActionInfo
    {
        [JsonProperty("action", Required = Required.Always)]
        public string Action { get; set; }

        [JsonProperty("time", Required = Required.Always)]
        public int Time { get; set; }
    }
}
