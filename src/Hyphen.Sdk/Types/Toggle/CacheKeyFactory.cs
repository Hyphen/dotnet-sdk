namespace Hyphen.Sdk;

/// <summary>
/// Represents a function which converts a toggle key and toggle context into a cache key
/// for evaluation caching purposes.
/// </summary>
/// <param name="toggleKey">The toggle key</param>
/// <param name="context">The optional toggle context</param>
/// <returns>The cache key; return <see langword="null"/> to indicate the value shouldn't be cached</returns>
/// <remarks>
/// Users can use this to override the default cache key construction.<br />
/// <br />
/// The default implementation lives in <see cref="DefaultCacheFactories.KeyFactory"/>.
/// </remarks>
public delegate string? CacheKeyFactory(string toggleKey, ToggleContext? context);
