using Newtonsoft.Json;

namespace StaffDaemon.Services.Slack
{
    public sealed class SlackUserResponse : SlackResponse
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
    }
}
