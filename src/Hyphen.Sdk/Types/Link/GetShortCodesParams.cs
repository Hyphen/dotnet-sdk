namespace Hyphen.Sdk;

/// <summary>
/// Represents optional parameters when calling <see cref="ILink.GetShortCodes"/>.
/// </summary>
public class GetShortCodesParams : PagedParams
{
	/// <summary>
	/// Gets or sets an optional search term filter.
	/// </summary>
	/// <remarks>
	/// Searches are performed across codes, URLs, and titles.
	/// </remarks>
	public string? Search { get; set; }

	/// <summary>
	/// Gets or sets an optional tag list filter for short code tags.
	/// </summary>
	public IReadOnlyCollection<string>? Tags { get; set; }
}
