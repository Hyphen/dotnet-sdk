using System.Diagnostics.CodeAnalysis;
using System.Text;
using Hyphen.Sdk.Resources;

namespace Hyphen.Sdk;

/// <summary>
/// Represents a project public key.
/// </summary>
public class ProjectPublicKey
{
	readonly string publicKey;

	/// <summary>
	/// Initializes a new instance of the <see cref="ProjectPublicKey"/> class.
	/// </summary>
	/// <param name="publicKey">The optional public key. If not provided, then the environment
	/// variable <c>HYPHEN_PROJECT_PUBLIC_KEY</c> will be used as the public key.</param>
	public ProjectPublicKey(string? publicKey = null)
	{
		publicKey ??= Environment.GetEnvironmentVariable(Env.ProjectPublicKey);

		if (string.IsNullOrWhiteSpace(publicKey))
			throw new PublicKeyException(HyphenSdkResources.ProjectPublicKey_Required);

		if (!publicKey.StartsWith("public_", StringComparison.OrdinalIgnoreCase))
			throw new PublicKeyException(HyphenSdkResources.ProjectPublicKey_MustBePublic);

		try
		{
#if NETSTANDARD
			var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(publicKey.Substring(7)));
#else
			var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(publicKey[7..]));
#endif
			var pieces = decoded.Split(':');
			if (pieces.Length < 3)
				throw new PublicKeyException(HyphenSdkResources.ProjectPublicKey_Malformed);

			OrganizationId = pieces[0];
			ProjectId = pieces[1];
		}
		catch
		{
			throw new PublicKeyException(HyphenSdkResources.ProjectPublicKey_Malformed);
		}

		this.publicKey = publicKey;
	}

	/// <summary>
	/// Gets the organization ID for this public key.
	/// </summary>
	public string OrganizationId { get; }

	/// <summary>
	/// Gets the project ID for this public key.
	/// </summary>
	public string ProjectId { get; }

	/// <summary>
	/// Gets the API key value.
	/// </summary>
	[ExcludeFromCodeCoverage]
	public string Value => publicKey;

	/// <summary>
	/// Create an instance of <see cref="ProjectPublicKey"/> from a <see cref="string"/>.
	/// </summary>
	[ExcludeFromCodeCoverage]
	public static ProjectPublicKey FromString(string? publicKey) => new(publicKey);

	/// <inheritdoc/>
	[ExcludeFromCodeCoverage]
	public override string ToString() => publicKey;

	/// <summary>
	/// Converts an <see cref="ProjectPublicKey"/> into a <see cref="string"/>.
	/// </summary>
	[ExcludeFromCodeCoverage]
	public static implicit operator string(ProjectPublicKey publicKey) => Guard.ArgumentNotNull(publicKey).publicKey;

	/// <summary>
	/// Converts a <see cref="string"/> into an <see cref="ProjectPublicKey"/>.
	/// </summary>
	[ExcludeFromCodeCoverage]
	public static implicit operator ProjectPublicKey(string? publicKey) => new(publicKey);
}
