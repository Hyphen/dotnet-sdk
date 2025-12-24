using System.Diagnostics.CodeAnalysis;

namespace Hyphen.Sdk;

/// <summary>
/// Represents the options values that can be used to configure <see cref="HyphenSdkServiceCollectionExtensions.AddToggle"/>.
/// </summary>
[ExcludeFromCodeCoverage]
public class ToggleOptions
{
	/// <summary>
	/// Gets or sets the optional application ID.
	/// </summary>
	/// <remarks>
	/// If the value is not set, and the user has set <c>HYPHEN_APP_ID</c>, the environment value will be
	/// used instead.
	/// </remarks>
	public string? ApplicationId { get; set; }

	/// <summary>
	/// Gets or sets the base URLs for the request. The URLs will be tried in order for load
	/// balancing and failover purposes.
	/// </summary>
	/// <remarks>
	/// If the value is not set, the service will default to <see href="https://{OrganizationId}.toggle.hyphen.cloud"/>
	/// and <see href="https://toggle.hyphen.cloud"/>.
	/// </remarks>
	public IReadOnlyCollection<Uri>? BaseUris { get; set; }

	/// <summary>
	/// Gets or sets the cache key algorithm used to determine how to cache responses.
	/// </summary>
	/// <remarks>
	/// if the value is not set, the default cache key is the first value value of:
	/// <list type="bullet">
	/// <item><c>"{toggleKey}-{context.User.Id}"</c></item> if the user ID is set
	/// <item><c>"{toggleKey}-{context.User.Email}"</c></item> if the user email is set
	/// <item>Last resort: <c>"{toggleKey}"</c></item>
	/// </list>
	/// </remarks>
	public CacheKeyFactory? CacheKeyFactory { get; set; }

	/// <summary>
	/// Gets or sets the cache policy used for caching toggle evaluation responses.
	/// </summary>
	/// <remarks>
	/// This factory will only be called once per toggle key, and the resulting policy will be reused
	/// every time that toggle key is requsted.<br />
	/// <br />
	/// If the policy factory returns <see langword="null"/>, then the value in question will not be cached.<br />
	/// <br />
	/// If there is no cache policy factory, then all requests will be cached for 30 seconds.
	/// </remarks>
	public CachePolicyFactory? CachePolicyFactory { get; set; }

	/// <summary>
	/// Gets or sets the default context used when making requests.
	/// </summary>
	/// <remarks>
	/// Some methods allow overriding the context. This value will be used when an override is not set.
	/// </remarks>
	public ToggleContext? DefaultContext { get; set; }

	/// <summary>
	/// Gets or sets the default targeting key if one cannot be derived from context.
	/// </summary>
	/// <remarks>
	/// The targeting key for an evaluation will be the first valid one of these in order:
	/// <list type="bullet">
	/// <item>The targeting key from the evaluation context, if present</item>
	/// <item>The user ID from the evaluation context, if present</item>
	/// <item>The value of this property, if present</item>
	/// <item>Last resort: <c>"{ApplicationId}-{Environment}-{RandomValue}"</c></item>
	/// </list>
	/// If the last resort value is used, the random value will change every time the application
	/// is launched.
	/// </remarks>
	public string? DefaultTargetingKey { get; set; }

	/// <summary>
	/// Gets or sets the environment (i.e., <c>"production"</c>, <c>"development"</c>, <c>"staging"</c>, etc.).
	/// </summary>
	/// <remarks>
	/// If this value is not set, the default value of <c>"development"</c> will be used.
	/// </remarks>
	public string? Environment { get; set; }

	/// <summary>
	/// Gets or sets the project public API key used to issue requests. The value must start with <c>"public_"</c>.
	/// </summary>
	/// <remarks>
	/// If the value is not set, the service class will assume that the user has set the environment variable
	/// <c>HYPHEN_PROJECT_PUBLIC_API_KEY</c> with the public API key. If neither value is set, the service
	/// class will throw an error about the missing public API key.
	/// </remarks>
	public string? ProjectPublicKey { get; set; }
}
