namespace Hyphen.Sdk;

/// <summary>
/// Represents a problem with a public key.
/// </summary>
/// <param name="message">The error message that explains the reason for the exception.</param>
[Serializable]
public class PublicKeyException(string message)
	: Exception(message)
{ }
