namespace StaffDaemon.Services.StaffVacations
{
    public class StaffVacationOptions
    {
        public int StaffNotifyDays { get; set; }
        public int LeaderNotifyDays { get; set; }
        public int HrDepartmentNotifyDays { get; set; }
        public string HrDepartmentSlackId { get; set; }
        public string VacationScheduleUrl { get; set; }
        public string WikiUrl { get; set; }
        public string SpreadsheetId { get; set; }
        public string[] SpreadsheetRanges { get; set; }
    }
}
