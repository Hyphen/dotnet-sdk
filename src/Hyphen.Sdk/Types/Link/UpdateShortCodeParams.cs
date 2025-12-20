namespace Hyphen.Sdk;

/// <summary>
/// Represents optional parameters when calling <see cref="ILink.UpdateShortCode"/>.
/// </summary>
public class UpdateShortCodeParams
{
	/// <summary>
	/// Gets or sets the URL that the short code link redirects to.
	/// </summary>
	/// <remarks>
	/// If this value is unset, the URL will not be changed.
	/// </remarks>
	public Uri? LongUrl { get; set; }

	/// <summary>
	/// Gets or sets the optional tags associated with the link.
	/// </summary>
	/// <remarks>
	/// If this value is unset, the tags will not be changed. To remove all the tags,
	/// set this property to empty array.
	/// </remarks>
	public IReadOnlyCollection<string>? Tags { get; set; }

	/// <summary>
	/// Gets or sets the optional title for the link.
	/// </summary>
	/// <remarks>
	/// If this value is unset, the title will not be changed.
	/// </remarks>
	public string? Title { get; set; }
}
