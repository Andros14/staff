using Microsoft.Extensions.Logging;

namespace StaffDaemon.Services.Common
{
    public abstract class BaseSeviceInitialParams
    {
        public BaseSeviceInitialParams(ILoggerFactory loggerFactory)
        {
            Logger = loggerFactory.CreateLogger(GetType().Name.Replace("InitialParams", string.Empty));
        }

        public ILogger Logger { get; }
    }
}
