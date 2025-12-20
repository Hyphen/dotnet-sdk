namespace Hyphen.Sdk;

/// <summary>
/// Represents results from a paged request.
/// </summary>
/// <typeparam name="T">The type of the results.</typeparam>
public class PagedResult<T>
{
	/// <summary>
	/// Gets or sets the retrieved data for this page.
	/// </summary>
	[JsonPropertyName("data")]
	public required T[] Data { get; set; }

	/// <summary>
	/// Gets or sets the page number for the paged retrieval request.
	/// </summary>
	[JsonPropertyName("pageNum")]
	public required int PageNumber { get; set; }

	/// <summary>
	/// Gets or sets the size of the results page.
	/// </summary>
	[JsonPropertyName("pageSize")]
	public required int PageSize { get; set; }

	/// <summary>
	/// Gets or sets the total number of data items available.
	/// </summary>
	[JsonPropertyName("total")]
	public required int Total { get; set; }
}
