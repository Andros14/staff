using StaffDaemon.Jobs.Common;

namespace StaffDaemon.Daemon
{
    public interface IDaemonJob
    {
        /// <summary>
        /// Название задачи
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Включена ли задача
        /// </summary>
        bool Enabled { get; }

        /// <summary>
        /// Время ежедневного запуска
        /// </summary>
        List<TimeSpan> DailyLaunchTimes { get; }

        /// <summary>
        /// Время ежемесячного запуска 
        /// </summary>
        TimeSpan MonthlyLaunchTime { get; }

        /// <summary>
        /// Дни ежемесячного запуска
        /// </summary>
        List<byte> MonthlyLaunchDays { get; }

        /// <summary>
        /// Время ожидания в секундах. 
        /// </summary>
        int WaitingTimeInSeconds { get; }

        /// <summary>
        /// Режим запуска
        /// </summary>
        ScheduleMode ScheduleMode { get; }

        /// <summary>
        /// Идентификатор пользователя в слаке, которому слать алерты
        /// </summary>
        string AlertUserSlackId { get; }

        /// <summary>
        /// Выполняет работу немедленно при старте
        /// </summary>
        /// <param name="cancellationToken">Токен отмены</param>
        Task ExecuteImmediateAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Выполняет работу по расписанию
        /// <param name="cancellationToken">Токен отмены</param>
        /// </summary>
        Task ExecuteAsync(CancellationToken cancellationToken);
    }
}
