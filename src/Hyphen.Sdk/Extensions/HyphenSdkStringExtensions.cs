using System.Diagnostics.CodeAnalysis;

namespace System;

[ExcludeFromCodeCoverage]
internal static class HyphenSdkStringExtensions
{
#if NETSTANDARD
	public static bool EndsWith(this string str, char value)
		=> str.Length != 0 && str[str.Length - 1] == value;
#endif

	public static string Quoted(this string? str) =>
		str is null ? "null" : $@"""{str}""";

	public static string ReplaceInvariant(this string str, string oldValue, string newValue)
#if NETSTANDARD
		=> str.Replace(oldValue, newValue);
#else
		=> str.Replace(oldValue, newValue, StringComparison.InvariantCulture);
#endif

#if NETSTANDARD
	public static bool StartsWith(this string str, char value)
		=> str.Length != 0 && str[0] == value;
#endif
}
