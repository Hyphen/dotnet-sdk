namespace Hyphen.Sdk;

/// <summary>
/// Represents an organization in an HTTP response.
/// </summary>
public class OrganizationResult
{
	/// <summary>
	/// Gets or sets the organization ID.
	/// </summary>
	[JsonPropertyName("id")]
	public required string Id { get; set; }

	/// <summary>
	/// Gets or sets the organization name.
	/// </summary>
	[JsonPropertyName("name")]
	public required string Name { get; set; }
}
