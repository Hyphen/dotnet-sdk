namespace Hyphen.Sdk;

/// <summary>
/// Represents a count of clicks for a specific day.
/// </summary>
public class LinkClicksByDay
{
	/// <summary>
	/// Gets or sets the date this click report represents.
	/// </summary>
	[JsonPropertyName("date")]
	public required DateTimeOffset Date { get; set; }

	/// <summary>
	/// Gets or sets the total number of clicks for the day.
	/// </summary>
	[JsonPropertyName("total")]
	public required long Total { get; set; }

	/// <summary>
	/// Gets or sets the number of unique users for the day.
	/// </summary>
	[JsonPropertyName("unique")]
	public required long Unique { get; set; }
}
