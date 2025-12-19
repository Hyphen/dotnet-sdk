using System.Diagnostics.CodeAnalysis;

namespace Hyphen.Sdk;

/// <summary>
/// Represents a problem with the API key.
/// </summary>
/// <param name="message">The error message that explains the reason for the exception.</param>
[Serializable]
[ExcludeFromCodeCoverage]
public class ApiKeyException(string message)
	: Exception(message)
{ }
