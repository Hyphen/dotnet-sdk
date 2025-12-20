namespace Hyphen.Sdk;

/// <summary>
/// Represents a collection of <see cref="LinkClicksByDay"/> objects.
/// </summary>
public class LinkClicks
{
	/// <summary>
	/// Gets or sets the list of clicks broken out by day.
	/// </summary>
	[JsonPropertyName("byDay")]
	public required LinkClicksByDay[] ByDay { get; set; }

	/// <summary>
	/// Gets or sets the total number of clicks across all reported days.
	/// </summary>
	[JsonPropertyName("total")]
	public required long Total { get; set; }

	/// <summary>
	/// Gets or sets the number of unique users across all reported days.
	/// </summary>
	[JsonPropertyName("unique")]
	public required long Unique { get; set; }
}
