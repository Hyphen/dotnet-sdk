namespace Hyphen.Sdk;

/// <summary>
/// Represents the result of getting, creating, or updating a short code link.
/// </summary>
public class ShortCodeResult
{
	/// <summary>
	/// Gets or sets the short code.
	/// </summary>
	[JsonPropertyName("code")]
	public required string Code { get; set; }

	/// <summary>
	/// Gets or sets the domain of the short code link.
	/// </summary>
	[JsonPropertyName("domain")]
	public required string Domain { get; set; }

	/// <summary>
	/// Gets or sets the ID of the short code.
	/// </summary>
	[JsonPropertyName("id")]
	public required string Id { get; set; }

	/// <summary>
	/// Gets or sets the URI that the short code points to.
	/// </summary>
	[JsonPropertyName("long_url")]
	public required Uri LongUrl { get; set; }

	/// <summary>
	/// Gets or sets information about the organization that owns the short code.
	/// </summary>
	[JsonPropertyName("organization")]
	public required OrganizationResult Organization { get; set; }

	/// <summary>
	/// Gets or sets the tags that are assocaited with the short code.
	/// </summary>
	[JsonPropertyName("tags")]
	public required string[] Tags { get; set; }

	/// <summary>
	/// Gets or sets the title of the short code.
	/// </summary>
	[JsonPropertyName("title")]
	public string? Title { get; set; }
}
