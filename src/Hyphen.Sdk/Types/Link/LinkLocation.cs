namespace Hyphen.Sdk;

/// <summary>
/// Represents a location that used a short code link.
/// </summary>
public class LinkLocation
{
	/// <summary>
	/// Gets or sets the country name.
	/// </summary>
	[JsonPropertyName("country")]
	public required string Country { get; set; }

	/// <summary>
	/// Gets or sets the total number of times the given country used the short code link.
	/// </summary>
	[JsonPropertyName("total")]
	public required int Total { get; set; }

	/// <summary>
	/// Gets or sets the number of unique users in the given country that used the short code link.
	/// </summary>
	[JsonPropertyName("unique")]
	public required int Unique { get; set; }
}
