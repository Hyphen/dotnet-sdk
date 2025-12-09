using System.Text.Json.Serialization;

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
	public required string IP { get; set; }

	/// <summary>
	/// Gets the IP address type.
	/// </summary>
	[JsonPropertyName("type")]
	public required IPType Type { get; set; }

	/// <summary>
	/// Gets the location information associated with the IP address.
	/// </summary>
	/// <remarks>
	/// This will only be set when <see cref="Type"/> is <see cref="IPType.Public"/>.
	/// </remarks>
	[JsonPropertyName("location")]
	public Location? Location { get; set; }

	/// <summary>
	/// Gets the error message.
	/// </summary>
	/// <remarks>
	/// This will only be set when <see cref="Type"/> is <see cref="IPType.Error"/>.
	/// </remarks>
	[JsonPropertyName("errorMessage")]
	public string? ErrorMessage { get; set; }
}
