namespace Hyphen.Sdk;

/// <summary>
/// Represents the result of getting statistics for a short code.
/// </summary>
public class ShortCodeStatsResult
{
	/// <summary>
	/// Gets or sets the browsers that used a short code link.
	/// </summary>
	[JsonPropertyName("browsers")]
	public required LinkBrowser[] Browsers { get; set; }

	/// <summary>
	/// Gets or sets the clicks that were recorded for the short code link.
	/// </summary>
	[JsonPropertyName("clicks")]
	public required LinkClicks Clicks { get; set; }

	/// <summary>
	/// Gets or sets the devices that used a short code link.
	/// </summary>
	[JsonPropertyName("devices")]
	public required LinkDevice[] Devices { get; set; }

	/// <summary>
	/// Gets or sets the locations that used a short code link.
	/// </summary>
	[JsonPropertyName("locations")]
	public required LinkLocation[] Locations { get; set; }

	/// <summary>
	/// Gets or sets the referrals that came from a short code link.
	/// </summary>
	[JsonPropertyName("referrals")]
	public required LinkReferral[] Referrals { get; set; }
}
