using Microsoft.Extensions.Logging;
using StaffDaemon.Daemon;
using StaffDaemon.PlatformUtils.Extensions;

namespace StaffDaemon.Jobs.Common
{
    public abstract class BaseJob<TParams, TOptions> : IDaemonJob
        where TParams : BaseJobInitialParams<TOptions>
        where TOptions : BaseJobOptions, new()
    {
        public BaseJob(TParams initialParams)
        {
            this.initialParams = initialParams;
        }

        protected TParams initialParams;

        public string Name => initialParams.Options.JobeName;
        public bool Enabled => initialParams.Options.Enabled;
        public List<TimeSpan> DailyLaunchTimes => initialParams.Options.DailyLaunchTimes
            .Split(';', StringSplitOptions.RemoveEmptyEntries)
            .Select(kv => kv.ToTimeSpan())
            .ToList();
        public TimeSpan MonthlyLaunchTime => initialParams.Options.MonthlyLaunchTime
            .ToTimeSpan();
        public List<byte> MonthlyLaunchDays => initialParams.Options.MonthlyLaunchDays
            .Split(';', StringSplitOptions.RemoveEmptyEntries)
            .Select(kv => kv.ConvertTo<byte>())
            .ToList();
        public int WaitingTimeInSeconds => initialParams.Options.WaitingTimeInSeconds
            .ConvertTo<int>();
        public ScheduleMode ScheduleMode => initialParams.Options.ScheduleMode;
        public string AlertUserSlackId => initialParams.Options.AlertUserSlackId;

        public bool CheckEnabled()
        {
            initialParams.Logger.LogDebug($"{Name} enabled: {Enabled}");
            return Enabled;
        }

        public bool CheckCancellationToken(CancellationToken cancellationToken)
        {
            initialParams.Logger.LogDebug($"{Name} CancellationToken: {cancellationToken.IsCancellationRequested}");
            return !cancellationToken.IsCancellationRequested;
        }

        public virtual async Task ExecuteImmediateAsync(CancellationToken cancellationToken)
        {
            if (ScheduleMode <= ScheduleMode.Period)
                await ExecuteAsync(cancellationToken);
        }

        public virtual async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                if (!CheckEnabled())
                    return;

                await ExecuteInternalAsync(cancellationToken);
            }
            catch (OperationCanceledException x)
            {
                initialParams.Logger.LogInformation(x, $"Stop of '{Name}'");
            }
            catch (Exception x)
            {
                await initialParams.SlackService.SendMessageAsync(Name, x.ToString(), AlertUserSlackId);
            }
        }

        protected abstract Task ExecuteInternalAsync(CancellationToken cancellationToken);
    }
}
