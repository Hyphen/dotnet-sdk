using System.Runtime.Caching;

namespace Hyphen.Sdk;

/// <summary>
/// Represents a function which converts a toggle key into a cache policy.
/// </summary>
/// <param name="toggleKey">The toggle key</param>
/// <returns>The cache policy; return <see langword="null"/> for uncached values.</returns>
/// <remarks>
/// Users can use this to override the default 30 second cache policy for all toggle investigations.<br />
/// <br />
/// The default implementation lives in <see cref="DefaultCacheFactories.PolicyFactory"/>.
/// </remarks>
public delegate CacheItemPolicy? CachePolicyFactory(string toggleKey);
