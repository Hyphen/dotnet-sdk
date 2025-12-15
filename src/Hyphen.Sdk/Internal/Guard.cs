using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Hyphen.Sdk.Internal;

[ExcludeFromCodeCoverage]
internal static class Guard
{
	public static T ArgumentNotNull<T>(
		[NotNull] T? argValue,
		[CallerArgumentExpression(nameof(argValue))] string? argName = null)
			where T : class =>
				argValue ?? throw new ArgumentNullException(argName?.TrimStart('@'));
}
