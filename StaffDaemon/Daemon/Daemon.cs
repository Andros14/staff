using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using FluentScheduler;
using StaffDaemon.Services.Slack;
using StaffDaemon.Jobs.Common;

namespace StaffDaemon.Daemon
{
    public class Daemon<TDaemonJob> : IHostedService where TDaemonJob : IDaemonJob
    {
        public Daemon(TDaemonJob job,
            ILoggerFactory loggerFactory,
            ISlackService slackService)
        {
            this.job = job;
            logger = loggerFactory.CreateLogger<Daemon<TDaemonJob>>();
            this.slackService = slackService;
            LockTimeout = TimeSpan.FromSeconds(10);
            InvalidJobRelaunchTimeout = TimeSpan.FromMinutes(10);
        }

        private readonly TDaemonJob job;
        private readonly ILogger<Daemon<TDaemonJob>> logger;
        private readonly ISlackService slackService;
        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
        private bool isPreviousConfigValid = true;

        public TimeSpan LockTimeout { get; set; }
        public TimeSpan InvalidJobRelaunchTimeout { get; set; }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (!IsValidJob())
            {
                logger.LogError($"Can't planed '{job.Name}'");
                await StopAndWaitForValidJobAsync(cancellationToken);
                return;
            }

            await TryExecuteImmediateAsync(cancellationToken);
            await PlanNextLaunchAsync(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                if (!JobManager.AllSchedules.Any(s => s.Name.Equals(job.Name)))
                    break;

                JobManager.RemoveJob(job.Name);
            }

            await Task.CompletedTask;
        }

        private async Task PlanNextLaunchAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation($"Start planning for '{job.Name}'");
            if (!IsValidJob())
            {
                logger.LogError($"Can't planed '{job.Name}'");
                await StopAndWaitForValidJobAsync(cancellationToken);
                return;
            }

            if (!isPreviousConfigValid)
            {
                await TryExecuteImmediateAsync(cancellationToken);
                isPreviousConfigValid = true;
            }

            await StopAsync(CancellationToken.None);
            var registry = new Registry();

            switch (job.ScheduleMode)
            {
                case ScheduleMode.Daily:
                    job.DailyLaunchTimes.ForEach(launchTime =>
                    {
                        registry.Schedule(async () => await PreExecuteAsync(cancellationToken))
                            .WithName(job.Name)
                            .ToRunEvery(1)
                            .Days()
                            .At(launchTime.Hours, launchTime.Minutes);
                    });
                    break;
                case ScheduleMode.Monthly:
                    job.MonthlyLaunchDays.ForEach(launchDay =>
                    {
                        registry.Schedule(async () => await PreExecuteAsync(cancellationToken))
                            .WithName(job.Name)
                            .ToRunEvery(1)
                            .Months()
                            .On(launchDay)
                            .At(job.MonthlyLaunchTime.Hours, job.MonthlyLaunchTime.Minutes);
                    });
                    break;
                case ScheduleMode.Delay:
                case ScheduleMode.Period:
                    registry.Schedule(async () => await PreExecuteAsync(cancellationToken))
                        .WithName(job.Name)
                        .ToRunOnceIn(job.WaitingTimeInSeconds)
                        .Seconds();
                    break;
                default:
                    await StopAndWaitForValidJobAsync(cancellationToken);
                    return;
            }

            JobManager.Initialize(registry);
            logger.LogInformation($"Finish planing for '{job.Name}'");
        }

        private async Task StopAndWaitForValidJobAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation($"Stop and wait for valid job '{job.Name}'");
            await StopAsync(CancellationToken.None);

            isPreviousConfigValid = false;

            var registry = new Registry();
            registry.Schedule(async () => await PreExecuteAsync(cancellationToken))
                .WithName(job.Name)
                .ToRunOnceIn((int)InvalidJobRelaunchTimeout.TotalSeconds)
                .Seconds();
            JobManager.Initialize(registry);
        }

        private async Task PreExecuteAsync(CancellationToken cancellationToken)
        {
            for (var i = 0; i < 5; i++)
            {
                if (await TryPreExecuteAsync(cancellationToken))
                    return;
            }

            var message = $"Can't PreExecute '{job.Name}'";
            await slackService.SendMessageAsync(message, message, job.AlertUserSlackId);
        }

        private async Task<bool> TryPreExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                switch (job.ScheduleMode)
                {
                    case ScheduleMode.Period:
                    case ScheduleMode.Daily:
                    case ScheduleMode.Monthly:
                        await PlanNextLaunchAsync(cancellationToken);
                        await TryExecuteAsync(cancellationToken);
                        return true;
                    case ScheduleMode.Delay:
                        await TryExecuteAsync(cancellationToken);
                        await PlanNextLaunchAsync(cancellationToken);
                        return true;
                    default:
                        await StopAndWaitForValidJobAsync(cancellationToken);
                        return true;
                }
            }
            catch (Exception x)
            {
                logger.LogError(x, $"Can't PreExecute '{job.Name}'");
                return false;
            }
        }

        private async Task TryExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                await semaphore.WaitAsync();
                await job.ExecuteAsync(cancellationToken);
            }
            catch (Exception x)
            {
                logger.LogError(x, $"Can't executed '{job.Name}'");
            }
            finally
            {
                semaphore.Release();
            }
        }

        private async Task TryExecuteImmediateAsync(CancellationToken cancellationToken)
        {
            try
            {
                await semaphore.WaitAsync();
                await job.ExecuteImmediateAsync(cancellationToken);
            }
            catch (Exception x)
            {
                logger.LogError(x, $"Can't immediate executed '{job.Name}'");
            }
            finally
            {
                semaphore.Release();
            }
        }

        private bool IsValidJob()
        {
            try
            {
                switch (job.ScheduleMode)
                {
                    case ScheduleMode.Period:
                    case ScheduleMode.Delay:
                        return job.WaitingTimeInSeconds > 0;
                    case ScheduleMode.Daily:
                        return job.DailyLaunchTimes.Any();
                    case ScheduleMode.Monthly:
                        return job.MonthlyLaunchDays.Any() && job.MonthlyLaunchTime > TimeSpan.MinValue;
                    default:
                        return false;
                }
            }
            catch (Exception x)
            {
                logger.LogError(x, $"Can't verify validity for'{job.Name}'");
                return false;
            }
        }
    }
}
