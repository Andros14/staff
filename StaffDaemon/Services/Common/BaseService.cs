using Microsoft.Extensions.Logging;

namespace StaffDaemon.Services.Common
{
    public abstract class BaseService<TParams> where TParams : BaseSeviceInitialParams
    {
        public BaseService(TParams initialParams)
        {
            this.initialParams = initialParams;
        }

        protected readonly TParams initialParams;

        protected void LogError(Exception exception)
        {
            initialParams.Logger.LogError(exception, string.Empty);
        }

        protected T ExecuteSafe<T>(Func<T> action)
        {
            try
            {
                return action();
            }
            catch (Exception x)
            {
                LogError(x);
                return default;
            }
        }

        protected void ExecuteSafe(Action action)
        {
            try
            {
                action();
            }
            catch (Exception x)
            {
                LogError(x);
            }
        }

        protected async Task<T> ExecuteSafeAsync<T>(Func<Task<T>> action)
        {
            try
            {
                return await action();
            }
            catch (Exception x)
            {
                LogError(x);
                return default;
            }
        }

        protected async Task ExecuteSafeAsync(Func<Task> action)
        {
            try
            {
                await action();
            }
            catch (Exception x)
            {
                LogError(x);
            }
        }
    }
}
