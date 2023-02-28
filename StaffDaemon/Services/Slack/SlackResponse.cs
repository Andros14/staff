using Newtonsoft.Json;

namespace StaffDaemon.Services.Slack
{
    public class SlackResponse
    {
        [JsonProperty(PropertyName = "ok")]
        public bool Ok { get; set; }
    }
}
