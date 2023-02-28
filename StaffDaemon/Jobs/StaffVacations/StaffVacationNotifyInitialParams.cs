using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StaffDaemon.Jobs.Common;
using StaffDaemon.Services.Slack;
using StaffDaemon.Services.StaffVacations;

namespace StaffDaemon.Jobs.StaffVacations
{
    public class StaffVacationNotifyInitialParams : BaseJobInitialParams<StaffVacationNotifyOptions>
    {
        public StaffVacationNotifyInitialParams(ILoggerFactory loggerFactory,
            ISlackService slackService,
            IConfiguration configuration,
            IStaffVacationService staffVacationService)
                : base(loggerFactory, slackService, configuration)
        {
            StaffVacationService = staffVacationService;
        }

        public IStaffVacationService StaffVacationService { get; }
    }
}
