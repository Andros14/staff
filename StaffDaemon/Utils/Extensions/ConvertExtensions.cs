using System.ComponentModel;
using System.Globalization;

namespace StaffDaemon.PlatformUtils.Extensions
{
    public static class ConvertExtensions
    {
        public static T ConvertTo<T>(this object value, CultureInfo cultureInfo = default)
        {
            if (value is T tValue)
                return tValue;

            if (value is null)
                return default(T);

            if (cultureInfo is null)
                cultureInfo = CultureInfo.CurrentCulture;

            try
            {
                var sourceType = value.GetType();
                var targetType = typeof(T);

                var sourceConverter = TypeDescriptor.GetConverter(sourceType);
                if (sourceConverter.CanConvertTo(targetType))
                    return (T)sourceConverter.ConvertTo(null, cultureInfo, value, targetType);

                var targetConverter = TypeDescriptor.GetConverter(targetType);
                return targetConverter.CanConvertFrom(sourceType)
                    ? (T)targetConverter.ConvertFrom(null, cultureInfo, value)
                    : default(T);
            }
            catch (InvalidCastException)
            {
                return default;
            }
        }
    }
}
