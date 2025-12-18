namespace Hyphen.Sdk;

/// <summary>
/// Indicates the type of the IP address.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<IPType>))]
public enum IPType
{
	/// <summary>
	/// The IP address request resulted in an error.
	/// </summary>
	Error,

	/// <summary>
	/// The IP address is a public IP. It should contain location information.
	/// </summary>
	Public,

	/// <summary>
	/// The IP address is a private IP. It will not contain location information.
	/// </summary>
	Private,
}
