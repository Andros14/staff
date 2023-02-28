using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StaffDaemon.Services.Common;
using StaffDaemon.Services.GoogleSheets;
using StaffDaemon.Services.Slack;

namespace StaffDaemon.Services.StaffVacations
{
    public class StaffVacationServiceInitialParams : BaseSeviceInitialParams
    {
        public StaffVacationServiceInitialParams(
            ILoggerFactory loggerFactory,
            IConfiguration configuration,
            IGoogleSheetsService googleSheetsService,
            ISlackService slackService)
                : base(loggerFactory)
        {
            Options = new StaffVacationOptions();
            configuration.GetSection(nameof(StaffVacationOptions)).Bind(Options);

            GoogleSheetsService = googleSheetsService;
            SlackService = slackService;
        }

        public StaffVacationOptions Options { get; }

        public IGoogleSheetsService GoogleSheetsService { get; }
        public ISlackService SlackService { get; }
    }
}
