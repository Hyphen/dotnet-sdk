namespace Hyphen.Sdk;

/// <summary>
/// Base class for pagination options for multi-item requests.
/// </summary>
public class PagedParams
{
	/// <summary>
	/// Gets or sets the page number for the request.
	/// </summary>
	/// <remarks>
	/// Value value: a positive integer.
	/// </remarks>
	public int? PageNumber
	{
		get => field;
		set
		{
			Guard.ArgumentValid(value is null || value >= 1, "Page number must be a positive integer", nameof(PageNumber));
			field = value;
		}
	}

	/// <summary>
	/// Gets or sets the page size for the request.
	/// </summary>
	/// <remarks>
	/// Valid value: a positive integer between <c>5</c> and <c>200</c>, inclusive.
	/// </remarks>
	public int? PageSize
	{
		get => field;
		set
		{
			Guard.ArgumentValid(value is null || (value >= 5 && value <= 200), "Page size must be between 5 and 200", nameof(PageSize));
			field = value;
		}
	}
}
