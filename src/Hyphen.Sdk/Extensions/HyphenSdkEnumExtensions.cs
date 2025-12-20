namespace Hyphen.Sdk;

internal static class HyphenSdkEnumExtensions
{
	public static string ToFormString(this QrCodeSize size) =>
		size switch
		{
			QrCodeSize.Small => "small",
			QrCodeSize.Medium => "medium",
			QrCodeSize.Large => "large",
			_ => throw new ArgumentException("Unknown QrCodeSize value: " + size.ToString()),
		};
}
