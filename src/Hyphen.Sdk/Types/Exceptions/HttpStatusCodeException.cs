using System.Net;
using Hyphen.Sdk.Resources;

namespace Hyphen.Sdk;

/// <summary>
/// Represents a problem with an HTTP request.
/// </summary>
/// <param name="statusCode">The HTTP status code</param>
/// <param name="message">The error message that explains the reason for the exception.</param>
[Serializable]
public class HttpStatusCodeException(HttpStatusCode statusCode, string message)
	: Exception(HyphenSdkResources.Http_StatusCodeError(statusCode, message))
{
	/// <summary>
	/// Initializes a new instance of the <see cref="HttpStatusCodeException"/> class.
	/// </summary>
	/// <param name="statusCode">The HTTP status code</param>
	public HttpStatusCodeException(HttpStatusCode statusCode) : this(statusCode, statusCode.ToString())
	{ }

	/// <summary>
	/// Gets the HTTP status code.
	/// </summary>
	public HttpStatusCode StatusCode => statusCode;
}
