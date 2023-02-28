using StaffDaemon.Jobs.Common;

namespace StaffDaemon.Jobs.StaffVacations
{
    public class StaffVacationNotify : BaseJob<StaffVacationNotifyInitialParams, StaffVacationNotifyOptions>
    {
        public StaffVacationNotify(StaffVacationNotifyInitialParams initialParams)
            : base(initialParams)
        {
        }

        protected override async Task ExecuteInternalAsync(CancellationToken cancellationToken)
        {
            await initialParams.StaffVacationService.NotifyStaffAsync();
        }
    }
}
