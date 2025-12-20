using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Hyphen.Sdk;

[ExcludeFromCodeCoverage]
internal static class Guard
{
	public static T ArgumentNotNull<T>([NotNull] T? argValue, [CallerArgumentExpression(nameof(argValue))] string? argName = null)
		where T : class =>
			argValue ?? throw new ArgumentNullException(argName?.TrimStart('@'));

	public static void ArgumentValid(bool test, string failureMessage, string argName)
	{
		if (!test)
			throw new ArgumentException(failureMessage, argName);
	}

	public static void True(bool test, string failureMessage)
	{
		if (!test)
			throw new InvalidOperationException(failureMessage);
	}
}
