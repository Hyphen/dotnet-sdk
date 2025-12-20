using System.Globalization;

namespace System;

internal static class HyphenSdkDateTimeOffsetExtensions
{
	public static string ToISO8601(this DateTimeOffset dateTime) =>
		dateTime.ToString(@"yyyy-MM-ddTHH\:mm\:ss.fff\Z", CultureInfo.InvariantCulture);
}
