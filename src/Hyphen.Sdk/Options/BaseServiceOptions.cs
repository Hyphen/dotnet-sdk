namespace Hyphen.Sdk;

/// <summary>
/// Represents the options values that can be used to configure any service derived
/// from <see cref="BaseService"/>.
/// </summary>
public class BaseServiceOptions
{
	/// <summary>
	/// Gets or sets the API key used to issue requests.
	/// </summary>
	/// <remarks>
	/// If the value is not set, the service class will assume that the user has set the environment
	/// variable <c>HYPHEN_API_KEY</c> with the API key. If neither value is set, the service class
	/// will throw an error about the missing API key.<br />
	/// <br />
	/// Note that public API keys (i.e., those starting with <c>"public_"</c>) are not supported.
	/// </remarks>
	public string? ApiKey { get; set; }
}
