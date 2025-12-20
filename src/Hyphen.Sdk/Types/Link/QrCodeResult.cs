using System.Diagnostics.CodeAnalysis;
using Hyphen.Sdk.Resources;

namespace Hyphen.Sdk;

/// <summary>
/// Represents the result of getting, creating, or updating a QR code.
/// </summary>
public class QrCodeResult
{
	const string QrCodeHeader = "data:image/png;base64,";

	/// <summary>
	/// Gets or sets the ID of the QR code.
	/// </summary>
	[JsonPropertyName("id")]
	public required string Id { get; set; }

	/// <summary>
	/// Gets or sets the URI that the QR code points to.
	/// </summary>
	[JsonPropertyName("qrLink")]
	public required Uri Link { get; set; }

	/// <summary>
	/// Gets or sets the QR code.
	/// </summary>
	/// <remarks>
	/// This value is a MIME representation of the QR code image. It will typically be in the format
	/// of <c>"data:image/png;base64,BASE64_ENCODED_IMAGE"</c>. The <see cref="GetQrCodeBytes"/> method
	/// can be used to retrieve the raw image data as bytes, and <see cref="SaveQrCode(string, CancellationToken)"/>
	/// method can be used to save the raw data to a local file.
	/// </remarks>
	[JsonPropertyName("qrCode")]
	public required string QrCode { get; set; }

	/// <summary>
	/// Gets or sets the optional title of the QR code.
	/// </summary>
	[JsonPropertyName("title")]
	public string? Title { get; set; }

	/// <summary>
	/// Gets the QR code image, in byte array form.
	/// </summary>
	public byte[] GetQrCodeBytes()
	{
		if (!QrCode.StartsWith(QrCodeHeader, StringComparison.InvariantCulture))
			throw new InvalidOperationException(HyphenSdkResources.Link_QrCodeInvalidFormat);

#if NETSTANDARD
		return Convert.FromBase64String(QrCode.Substring(QrCodeHeader.Length));
#else
		return Convert.FromBase64String(QrCode[QrCodeHeader.Length..]);
#endif
	}

	/// <summary>
	/// Saves the QR code image to disk.
	/// </summary>
	/// <param name="fileName">The file name to save the QR code image to.</param>
	/// <remarks>
	/// The QR code images are in PNG format, so it is recommended that the file name end with <c>".png"</c>.
	/// </remarks>
	[ExcludeFromCodeCoverage]
	public Task SaveQrCode(string fileName) =>
		SaveQrCode(fileName, default);

	/// <summary>
	/// Saves the QR code image to disk.
	/// </summary>
	/// <param name="fileName">The file name to save the QR code image to.</param>
	/// <param name="cancellationToken">The cancellation token used to cancel the file write early.</param>
	/// <remarks>
	/// The QR code images are in PNG format, so it is recommended that the file name end with <c>".png"</c>.
	/// </remarks>
	[ExcludeFromCodeCoverage]  // Not testing due to file I/O (and simple implementation)
	public Task SaveQrCode(string fileName, CancellationToken cancellationToken)
	{
#if NETSTANDARD
		cancellationToken.ThrowIfCancellationRequested();
		File.WriteAllBytes(fileName, GetQrCodeBytes());
		return Task.CompletedTask;
#else
		return File.WriteAllBytesAsync(fileName, GetQrCodeBytes(), cancellationToken);
#endif
	}
}
