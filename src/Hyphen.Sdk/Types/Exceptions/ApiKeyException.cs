using System.Diagnostics.CodeAnalysis;

namespace Hyphen.Sdk;

/// <summary>
/// Represents a problem with the API key.
/// </summary>
/// <param name="message">The error message that explains the reason for the exception.</param>
/// <param name="inner">The exception that is the cause of the current exception, or a <c>null</c>
/// reference if no inner exception is specified.</param>
[Serializable]
[ExcludeFromCodeCoverage]
public class ApiKeyException(string? message, Exception? inner)
	: Exception(message, inner)
{
	/// <summary>
	/// Initializes a new instance of the <see cref="ApiKeyException"/> class.
	/// </summary>
	public ApiKeyException() : this(null, null)
	{ }

	/// <summary>
	/// Initializes a new instance of the <see cref="ApiKeyException"/> class with
	/// a specified error message.
	/// </summary>
	/// <param name="message">The error message that explains the reason for the exception.</param>
	public ApiKeyException(string? message) : this(message, null)
	{ }
}
