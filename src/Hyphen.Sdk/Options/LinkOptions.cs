namespace Hyphen.Sdk;

/// <summary>
/// Represents the options values that can be used to configure <see cref="HyphenSdkServiceCollectionExtensions.AddLink"/>.
/// </summary>
public class LinkOptions : BaseHttpServiceOptions
{
	/// <summary>
	/// Gets or sets the base URI template for the request.
	/// </summary>
	/// <remarks>
	/// If the value is not set, the service will default to <c>"https://api.hyphen.ai/api/organizations/{organizationId}/link/codes/"</c>.<br />
	/// <br/>
	/// Any URI template will have the pattern <c>{organizationId}</c> replaced with the organization ID during the request.
	/// </remarks>
#pragma warning disable CA1056  // This is a template for a URI, not an actual URI
	public string? BaseUriTemplate { get; set; }
#pragma warning restore CA1056

	/// <summary>
	/// Gets or sets the organization ID.
	/// </summary>
	/// <remarks>
	/// If the value is not set, the service class will assume that the user has set the environment
	/// variable <c>HYPHEN_ORGANIZATION_ID</c> with the organization ID. If neither value is set, the
	/// service class will throw an error about the missing organization ID.
	/// </remarks>
	public string? OrganizationId { get; set; }
}
