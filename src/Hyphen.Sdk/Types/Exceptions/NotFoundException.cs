namespace Hyphen.Sdk;

/// <summary>
/// Represents an item not found.
/// </summary>
/// <remarks>
/// This exception will only be thrown for update operations like <c>POST</c>, <c>PUT</c>, or <c>PATCH</c>.<br />
/// <c>GET</c> operations which result in a 404 will typically return <see langword="null"/> values.<br />
/// <c>DELETE</c> operations which result in a 404 will typically be silently ignored.
/// </remarks>
/// <param name="message">The error message that explains the reason for the exception.</param>
[Serializable]
public class NotFoundException(string message)
	: Exception(message)
{ }
