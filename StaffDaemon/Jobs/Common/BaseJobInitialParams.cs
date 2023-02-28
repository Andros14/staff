using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StaffDaemon.Services.Slack;

namespace StaffDaemon.Jobs.Common
{
    public abstract class BaseJobInitialParams<TOptions> where TOptions : BaseJobOptions, new()
    {
        public BaseJobInitialParams(ILoggerFactory loggerFactory,
            ISlackService slackService,
            IConfiguration configuration)
        {
            Options = new TOptions();
            configuration.GetSection(Options.ClassName).Bind(Options);

            Logger = loggerFactory.CreateLogger(Options.JobeName);

            SlackService = slackService;
        }

        public TOptions Options { get; }

        public ILogger Logger { get; }

        public ISlackService SlackService { get; }
    }
}
