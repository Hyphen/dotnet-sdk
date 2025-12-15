using System.Diagnostics.CodeAnalysis;

namespace Hyphen.Sdk;

/// <summary>
/// Represents the options values that can be used to configure <see cref="NetInfo"/>.
/// </summary>
[ExcludeFromCodeCoverage]
public class NetInfoOptions : BaseServiceOptions
{
	/// <summary>
	/// Gets or sets the base URL for the request.
	/// </summary>
	/// <remarks>
	/// If the value is not set, the service will default to <see href="https://net.info"/>.
	/// </remarks>
	public Uri? BaseUri { get; set; }
}
