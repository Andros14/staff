using Newtonsoft.Json;
using RestSharp.Serializers;

namespace StaffDaemon.PlatformUtils.Helpers.RestSharp
{
    public class CustomJsonSerializer : ISerializer
    {
        public CustomJsonSerializer()
        {
            Settings = new JsonSerializerSettings();
            ContentType = "application/json";
        }

        public string ContentType { get; set; }

        public JsonSerializerSettings Settings { get; set; }

        public string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj, Settings);
        }
    }
}
