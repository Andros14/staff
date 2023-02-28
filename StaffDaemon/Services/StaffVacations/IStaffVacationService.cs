namespace StaffDaemon.Services.StaffVacations
{
    public interface IStaffVacationService
    {
        /// <summary>
        ///Оповещает персонал об отпусках
        /// </summary>
        Task NotifyStaffAsync();
    }
}
