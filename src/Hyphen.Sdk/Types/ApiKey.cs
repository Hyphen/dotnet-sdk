using System.Diagnostics.CodeAnalysis;
using Hyphen.Sdk.Internal;
using Hyphen.Sdk.Resources;

namespace Hyphen.Sdk;

/// <summary>
/// Represents an API key.
/// </summary>
public class ApiKey
{
	readonly string apiKey;

	/// <summary>
	/// Initializes a new instance of the <see cref="ApiKey"/> class.
	/// </summary>
	/// <param name="apiKey">The optional API key. If not provided, then the environment
	/// variable <c>HYPHEN_API_KEY</c> will be used as the API key.</param>
	public ApiKey(string? apiKey = null)
	{
		apiKey ??= Environment.GetEnvironmentVariable(Env.ApiKey);

		if (string.IsNullOrWhiteSpace(apiKey))
			throw new ArgumentException(HyphenSdkResources.ApiKey_Required, nameof(apiKey));

		if (apiKey.StartsWith("public_", StringComparison.OrdinalIgnoreCase))
			throw new ArgumentException(HyphenSdkResources.ApiKey_ShouldNotBePublic, nameof(apiKey));

		this.apiKey = apiKey;
	}

	/// <summary>
	/// Create an instance of <see cref="ApiKey"/> from a <see cref="string"/>.
	/// </summary>
	[ExcludeFromCodeCoverage]
	public static ApiKey FromString(string apiKey) => new(apiKey);

	/// <inheritdoc/>
	[ExcludeFromCodeCoverage]
	public override string ToString() => apiKey;

	/// <summary>
	/// Converts an <see cref="ApiKey"/> into a <see cref="string"/>.
	/// </summary>
	[ExcludeFromCodeCoverage]
	public static implicit operator string(ApiKey apiKey) => Guard.ArgumentNotNull(apiKey).apiKey;

	/// <summary>
	/// Converts a <see cref="string"/> into an <see cref="ApiKey"/>.
	/// </summary>
	[ExcludeFromCodeCoverage]
	public static implicit operator ApiKey(string apiKey) => new(apiKey);
}
