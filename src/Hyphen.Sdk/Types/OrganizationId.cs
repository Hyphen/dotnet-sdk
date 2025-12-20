using System.Diagnostics.CodeAnalysis;
using Hyphen.Sdk.Resources;

namespace Hyphen.Sdk;

/// <summary>
/// Represents an organization ID.
/// </summary>
public class OrganizationId
{
	readonly string organizationId;

	/// <summary>
	/// Initializes a new instance of the <see cref="OrganizationId"/> class.
	/// </summary>
	/// <param name="organizationId">The optional organization ID. If not provided, then the environment
	/// variable <c>HYPHEN_ORGANIZATION_ID</c> will be used as the organization ID.</param>
	public OrganizationId(string? organizationId = null)
	{
		organizationId ??= Environment.GetEnvironmentVariable(Env.OrganizationId);

		if (string.IsNullOrWhiteSpace(organizationId))
			throw new ArgumentException(HyphenSdkResources.OrganizationId_Required);

		this.organizationId = organizationId;
	}

	/// <summary>
	/// Gets the API key value.
	/// </summary>
	[ExcludeFromCodeCoverage]
	public string Value => organizationId;

	/// <summary>
	/// Create an instance of <see cref="OrganizationId"/> from a <see cref="string"/>.
	/// </summary>
	[ExcludeFromCodeCoverage]
	public static OrganizationId FromString(string? organizationId) => new(organizationId);

	/// <inheritdoc/>
	[ExcludeFromCodeCoverage]
	public override string ToString() => organizationId;

	/// <summary>
	/// Converts an <see cref="OrganizationId"/> into a <see cref="string"/>.
	/// </summary>
	public static implicit operator string(OrganizationId organizationId) => Guard.ArgumentNotNull(organizationId).organizationId;

	/// <summary>
	/// Converts a <see cref="string"/> into an <see cref="organizationId"/>.
	/// </summary>
	public static implicit operator OrganizationId(string? organizationId) => new(organizationId);
}
