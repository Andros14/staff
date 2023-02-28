using Newtonsoft.Json;

namespace StaffDaemon.Services.Slack
{
    public sealed class SlackMessage
    {
        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "username")]
        public string UserName { get; set; }

        [JsonProperty(PropertyName = "channel")]
        public string Channel { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
