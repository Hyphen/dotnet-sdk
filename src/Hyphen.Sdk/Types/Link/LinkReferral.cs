namespace Hyphen.Sdk;

/// <summary>
/// Represents a referral for a short code link.
/// </summary>
public class LinkReferral
{
	/// <summary>
	/// Gets or sets the total number of times a referral from <see cref="Url"/> occurred.
	/// </summary>
	[JsonPropertyName("total")]
	public required long Total { get; set; }

	/// <summary>
	/// Gets or sets the URL of the referral.
	/// </summary>
	[JsonPropertyName("url")]
	public required Uri Url { get; set; }
}
