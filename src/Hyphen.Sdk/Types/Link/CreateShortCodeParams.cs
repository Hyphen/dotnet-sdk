namespace Hyphen.Sdk;

/// <summary>
/// Represents optional parameters when calling <see cref="ILink.CreateShortCode"/>.
/// </summary>
public class CreateShortCodeParams
{
	/// <summary>
	/// Gets or sets the short code used for the link.
	/// </summary>
	/// <remarks>
	/// If this value is unset, a random code will be generated.
	/// </remarks>
	public string? Code { get; set; }

	/// <summary>
	/// Gets or sets the optional tags associated with the link.
	/// </summary>
	public IReadOnlyCollection<string>? Tags { get; set; }

	/// <summary>
	/// Gets or sets the optional title for the link.
	/// </summary>
	public string? Title { get; set; }
}
