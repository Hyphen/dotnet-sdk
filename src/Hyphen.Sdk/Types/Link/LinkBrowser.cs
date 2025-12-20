namespace Hyphen.Sdk;

/// <summary>
/// Represents a browser used by a short code link.
/// </summary>
public class LinkBrowser
{
	/// <summary>
	/// The name of the browser.
	/// </summary>
	[JsonPropertyName("name")]
	public required string Name { get; set; }

	/// <summary>
	/// The total number of times the browser was seen.
	/// </summary>
	[JsonPropertyName("total")]
	public required long Total { get; set; }
}
