namespace System.Drawing;

internal static class HyphenSdkColorExtensions
{
	public static string ToCss(this Color color)
	{
		var result = "#"
			+ ToHexChar(color.R >> 4) + ToHexChar(color.R & 0xF)
			+ ToHexChar(color.G >> 4) + ToHexChar(color.G & 0xF)
			+ ToHexChar(color.B >> 4) + ToHexChar(color.B & 0xF);

		if (color.A != 255)
			result = result + ToHexChar(color.A >> 4) + ToHexChar(color.A & 0xF);

		return result;
	}

	static char ToHexChar(int b) => (char)(b < 10 ? b + '0' : b - 10 + 'a');
}
