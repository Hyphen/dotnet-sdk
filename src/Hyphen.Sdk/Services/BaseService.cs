using System.Diagnostics.CodeAnalysis;
using Hyphen.Sdk.Internal;

namespace Hyphen.Sdk;

/// <summary>
/// Represents a base class used for all service classes.
/// </summary>
/// <param name="httpClientFactory">The factory for creating <see cref="HttpClient"/> instances</param>
/// <param name="logger">The logger to be used by the service class</param>
/// <param name="options">The base service options</param>
public abstract class BaseService(IHttpClientFactory httpClientFactory, ILogger logger, IOptions<BaseServiceOptions> options)
{
	/// <summary>
	/// A list of case-insensitive values that indicate a value is falsey.
	/// </summary>
	static internal readonly HashSet<string?> FalseValues = new(StringComparer.OrdinalIgnoreCase) { "false", "off", "no", "0" };

	/// <summary>
	/// A list of case-insensitive values that indicate a value is truthy.
	/// </summary>
	static internal readonly HashSet<string?> TrueValues = new(StringComparer.OrdinalIgnoreCase) { "true", "on", "yes", "1" };

	/// <summary>
	/// Gets the API key used to make service requests.
	/// </summary>
	[ExcludeFromCodeCoverage]
	protected ApiKey ApiKey { get; } = new(Guard.ArgumentNotNull(options).Value.ApiKey);

	/// <summary>
	/// Gets the HTTP client factory used to make HTTP clients for service requests.
	/// </summary>
	[ExcludeFromCodeCoverage]
	protected IHttpClientFactory HttpClientFactory { get; } = Guard.ArgumentNotNull(httpClientFactory);

	/// <summary>
	/// Gets a flag indicating whether environment variable <c>HYPHEN_DEV</c> is truthy.
	/// </summary>
	[ExcludeFromCodeCoverage]
	internal static bool IsDevEnvironment => TrueValues.Contains(Environment.GetEnvironmentVariable(HyphenEnv.Dev));

	/// <summary>
	/// Gets the logger used to log messages.
	/// </summary>
	[ExcludeFromCodeCoverage]
	protected ILogger Logger { get; } = Guard.ArgumentNotNull(logger);
}
