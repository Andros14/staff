using System.Globalization;
using Newtonsoft.Json;
using RestSharp.Serializers;
using StaffDaemon.PlatformUtils.Helpers.RestSharp;

namespace StaffDaemon.PlatformUtils.Helpers
{
    public static class RestSharpHelper
    {
        /// <summary>
        /// Возвращает Newtonsoft'овскую реализацию Serializer'а для RestSharp-клиета
        /// </summary>
        /// <returns></returns>
        public static ISerializer GetJsonSerializer()
        {
            return new CustomJsonSerializer()
            {
                Settings = new JsonSerializerSettings()
                {
                    FloatParseHandling = FloatParseHandling.Decimal,
                    Culture = CultureInfo.InvariantCulture,
                    NullValueHandling = NullValueHandling.Ignore
                }
            };
        }
    }
}
