using System.Diagnostics.CodeAnalysis;

namespace Hyphen.Sdk;

/// <summary>
/// Represents the return value from <see cref="INetInfo"/>.
/// </summary>
public class NetInfoResult
{
	/// <summary>
	/// Gets the IP address of the request.
	/// </summary>
	[JsonPropertyName("ip")]
	[ExcludeFromCodeCoverage]
	public required string IP { get; set; }

	/// <summary>
	/// Gets the IP address type.
	/// </summary>
	[JsonPropertyName("type")]
	[ExcludeFromCodeCoverage]
	public required IPType Type { get; set; }

	/// <summary>
	/// Gets the location information associated with the IP address.
	/// </summary>
	/// <remarks>
	/// This will only be set when <see cref="Type"/> is <see cref="IPType.Public"/>.
	/// </remarks>
	[JsonPropertyName("location")]
	[ExcludeFromCodeCoverage]
	public Location? Location { get; set; }

	/// <summary>
	/// Gets the error message.
	/// </summary>
	/// <remarks>
	/// This will only be set when <see cref="Type"/> is <see cref="IPType.Error"/>.
	/// </remarks>
	[JsonPropertyName("errorMessage")]
	[ExcludeFromCodeCoverage]
	public string? ErrorMessage { get; set; }
}
