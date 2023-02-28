using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace StaffDaemon.PlatformUtils.Extensions
{
    public static class StringExtensions
    {
        public static bool IsEmpty(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        public static bool IsNotEmpty(this string value)
        {
            return !value.IsEmpty();
        }

        public static bool IsNotEqual(this string value, string other)
        {
            return !value.IsEqual(other);
        }

        public static bool IsEqual(this string value, string other)
        {
            return (value is null && other is null) || (value is not null && string.Equals(value, other, StringComparison.Ordinal));
        }

        public static TimeSpan ToTimeSpan(this string value)
        {
            if (!TimeSpan.TryParseExact(value, new[] { @"hh\:mm\:ss", @"hh\:mm" }, CultureInfo.InvariantCulture, out var result))
                throw new FormatException($"Incorrect time format {value}, need format 'HH:mm[:ss]'");

            return result;
        }
    }
}
