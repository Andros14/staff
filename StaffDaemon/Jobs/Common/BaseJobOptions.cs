namespace StaffDaemon.Jobs.Common
{
    public abstract class BaseJobOptions
    {
        public bool Enabled { get; set; }
        public string DailyLaunchTimes { get; set; }
        public string MonthlyLaunchTime { get; set; }
        public string MonthlyLaunchDays { get; set; }
        public string WaitingTimeInSeconds { get; set; }
        public ScheduleMode ScheduleMode { get; set; }
        public string AlertUserSlackId { get; set; }

        public string JobeName => ClassName.Replace("Options", "");


        public abstract string ClassName { get; }
    }
}
