using Hyphen.Sdk.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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
	/// Gets the API key used to make service requests.
	/// </summary>
	protected string ApiKey { get; } = Guard.ArgumentNotNull(options).Value.ApiKey;

	/// <summary>
	/// Gets the HTTP client factory used to make HTTP clients for service requests.
	/// </summary>
	protected IHttpClientFactory HttpClientFactory { get; } = Guard.ArgumentNotNull(httpClientFactory);

	/// <summary>
	/// Gets the logger used to log messages.
	/// </summary>
	protected ILogger Logger { get; } = Guard.ArgumentNotNull(logger);
}
