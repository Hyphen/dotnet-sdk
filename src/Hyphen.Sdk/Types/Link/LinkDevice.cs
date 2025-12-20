namespace Hyphen.Sdk;

/// <summary>
/// Represents a device type that used a short code link.
/// </summary>
public class LinkDevice
{
	/// <summary>
	/// Gets or sets the name of the device type.
	/// </summary>
	[JsonPropertyName("name")]
	public required string Name { get; set; }

	/// <summary>
	/// Gets or sets the total number of times the device type used the short link code.
	/// </summary>
	[JsonPropertyName("total")]
	public required int Total { get; set; }
}
