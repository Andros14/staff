using RestSharp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using StaffDaemon.Services.Common;

namespace StaffDaemon.Services.Slack
{
    public class SlackServiceInitialParams : BaseSeviceInitialParams
    {
        public SlackServiceInitialParams(ILoggerFactory loggerFactory, IConfiguration configuration)
            : base(loggerFactory)
        {
            Options = new SlackOptions();
            configuration.GetSection(nameof(SlackOptions)).Bind(Options);

            RestClient = new RestClient(Options.ApiUrl);
        }

        public SlackOptions Options { get; }
        public RestClient RestClient { get; }
    }
}
