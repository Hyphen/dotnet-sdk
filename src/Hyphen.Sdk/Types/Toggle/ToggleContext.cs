namespace Hyphen.Sdk;

/// <summary>
/// Represents the context for Toggle request evaluation.
/// </summary>
public class ToggleContext
{
	/// <summary>
	/// Gets or sets custom attributes for the context.
	/// </summary>
	[JsonPropertyName("customAttributes")]
	public IReadOnlyDictionary<string, object?>? CustomAttributes { get; set; }

	/// <summary>
	/// Gets or sets the IP address for evaluation.
	/// </summary>
	[JsonPropertyName("ipAddress")]
	public string? IPAddress { get; set; }

	/// <summary>
	/// Gets or sets the targeting key for evaluation.
	/// </summary>
	[JsonPropertyName("targetingKey")]
	public string? TargetingKey { get; set; }

	/// <summary>
	/// Gets or sets the user for evaluation.
	/// </summary>
	[JsonPropertyName("user")]
	public ToggleUser? User { get; set; }
}
