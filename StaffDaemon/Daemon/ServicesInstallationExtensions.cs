using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StaffDaemon.Jobs.Common;
using StaffDaemon.Jobs.StaffVacations;
using StaffDaemon.Services.GoogleSheets;
using StaffDaemon.Services.Slack;
using StaffDaemon.Services.StaffVacations;

namespace StaffDaemon.Daemon
{
    public static class ServicesInstallationExtensions
    {
        public static void InstallServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(configuration);

            services.AddSingleton<IGoogleSheetsService, GoogleSheetsService>();

            services.AddSingleton<ISlackService, SlackService>();
            services.AddSingleton<SlackServiceInitialParams>();

            services.AddSingleton<IStaffVacationService, StaffVacationService>();
            services.AddSingleton<StaffVacationServiceInitialParams>();
        }

        public static void InstallJobs(this IServiceCollection services)
        {
            services.AddJob<StaffVacationNotify, StaffVacationNotifyInitialParams, StaffVacationNotifyOptions>();
        }

        private static void AddJob<TJob, TInitialParams, TOptions>(this IServiceCollection services)
            where TJob : BaseJob<TInitialParams, TOptions>
            where TInitialParams : BaseJobInitialParams<TOptions>
            where TOptions : BaseJobOptions, new()
        {
            services.AddScoped<TInitialParams>();
            services.AddScoped<TJob>();
            services.AddHostedService<Daemon<TJob>>();
        }
    }
}
