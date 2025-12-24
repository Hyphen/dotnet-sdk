using System.Runtime.Caching;

namespace Hyphen.Sdk;

/// <summary>
/// Represents the default cache factory implementations.
/// </summary>
public static class DefaultCacheFactories
{
	static readonly CacheItemPolicy DefaultCacheItemPolicy = new() { SlidingExpiration = TimeSpan.FromSeconds(30) };

	/// <summary>
	/// The default implementation of <see cref="CacheKeyFactory"/>.
	/// </summary>
	/// <remarks>
	/// Returns a cache key which is the first value value of:
	/// <list type="bullet">
	/// <item><c>"{toggleKey}-{context.User.Id}"</c></item> if the user ID is set
	/// <item><c>"{toggleKey}-{context.User.Email}"</c></item> if the user email is set
	/// <item><c>"{toggleKey}"</c></item>
	/// </list>
	/// </remarks>
	public static string KeyFactory(string toggleKey, ToggleContext? context) =>
		context?.User?.CacheKey is not null
			? $"{toggleKey}-{context.User.CacheKey}"
			: toggleKey;

	/// <summary>
	/// The default implementation of <see cref="CachePolicyFactory"/>.
	/// </summary>
	public static CacheItemPolicy? PolicyFactory(string _) => DefaultCacheItemPolicy;
}
