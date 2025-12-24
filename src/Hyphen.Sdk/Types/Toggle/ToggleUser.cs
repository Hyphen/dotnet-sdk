namespace Hyphen.Sdk;

/// <summary>
/// Represents user information for evaluation.
/// </summary>
public class ToggleUser
{
	internal string? CacheKey => Id ?? Email;

	/// <summary>
	/// Gets or sets custom attributes for the user.
	/// </summary>
	[JsonPropertyName("customAttributes")]
	public IReadOnlyDictionary<string, object?>? CustomAttributes { get; set; }

	/// <summary>
	/// Gets or sets the user's email address.
	/// </summary>
	[JsonPropertyName("email")]
	public string? Email { get; set; }

	/// <summary>
	/// Gets or sets the user's ID.
	/// </summary>
	[JsonPropertyName("id")]
	public string? Id { get; set; }

	/// <summary>
	/// Gets or sets the user's name.
	/// </summary>
	[JsonPropertyName("name")]
	public string? Name { get; set; }
}
