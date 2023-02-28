using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;
using StaffDaemon.Daemon;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

var builder = new HostBuilder().ConfigureServices(services =>
{

    services.InstallServices(configuration);
    services.InstallJobs();
})
.ConfigureLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConfiguration(configuration.GetSection("Logging"));
})
.UseNLog();

await builder.RunConsoleAsync();
