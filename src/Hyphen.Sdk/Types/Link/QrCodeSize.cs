namespace Hyphen.Sdk;

/// <summary>
/// Represents the size of a QR code.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<QrCodeSize>))]
public enum QrCodeSize
{
	/// <summary>
	/// Represents a small-sized QR code.
	/// </summary>
	Small,

	/// <summary>
	/// Represents a medium-sized QR code.
	/// </summary>
	Medium,

	/// <summary>
	/// Represents a large-sized QR code.
	/// </summary>
	Large,
}
