using System.Drawing;

namespace Hyphen.Sdk;

/// <summary>
/// Represents optional parameters when calling <see cref="ILink.CreateQrCode"/>.
/// </summary>
public class CreateQrCodeParams
{
	/// <summary>
	/// Gets or sets the background color for the QR code.
	/// </summary>
	public Color? BackgroundColor { get; set; }

	/// <summary>
	/// Gets or sets the foreground color for the QR code.
	/// </summary>
	public Color? Color { get; set; }

	/// <summary>
	/// Gets or sets the logo for the QR code.
	/// </summary>
	/// <remarks>
	/// The bytes here should be the contents of the image file, which can be read from disk
	/// with <see cref="File.ReadAllBytes"/>.<br/>
	/// <br/>
	/// Supported formats include:
	/// <list type="bullet">
	/// <item><c>.avif</c></item>
	/// <item><c>.bmp</c></item>
	/// <item><c>.dds</c></item>
	/// <item><c>.exr</c> (OpenEXR)</item>
	/// <item><c>.ff</c> (Farbfeld)</item>
	/// <item><c>.gif</c></item>
	/// <item><c>.hdr</c></item>
	/// <item><c>.ico</c></item>
	/// <item><c>.jpeg</c></item>
	/// <item><c>.png</c></item>
	/// <item><c>.pnm</c></item>
	/// <item><c>.quo</c></item>
	/// <item><c>.tiff</c></item>
	/// <item><c>.webp</c></item>
	/// </list>
	/// </remarks>
	public byte[]? Logo { get; set; }

	/// <summary>
	/// Gets or sets the size of the generated QR code.
	/// </summary>
	public QrCodeSize? Size { get; set; }

	/// <summary>
	/// Gets or sets the title of the QR code.
	/// </summary>
	public string? Title { get; set; }
}
