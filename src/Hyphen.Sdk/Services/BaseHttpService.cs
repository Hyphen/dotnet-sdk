using System.Diagnostics.CodeAnalysis;
using Hyphen.Sdk.Internal;

namespace Hyphen.Sdk;

/// <summary>
/// Represents a base class used for all service classes which make HTTP requests to Hyphen.
/// </summary>
/// <param name="httpClientFactory">The factory for creating <see cref="HttpClient"/> instances</param>
/// <param name="logger">The logger to be used by the service class</param>
/// <param name="options">The base service options</param>
public abstract class BaseHttpService(IHttpClientFactory httpClientFactory, ILogger logger, IOptions<BaseHttpServiceOptions> options)
	: BaseService(logger)
{
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
}
