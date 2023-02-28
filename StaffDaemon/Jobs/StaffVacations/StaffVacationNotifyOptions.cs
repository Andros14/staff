using StaffDaemon.Jobs.Common;

namespace StaffDaemon.Jobs.StaffVacations
{
    public sealed class StaffVacationNotifyOptions : BaseJobOptions
    {
        public override string ClassName => nameof(StaffVacationNotifyOptions);
    }
}
