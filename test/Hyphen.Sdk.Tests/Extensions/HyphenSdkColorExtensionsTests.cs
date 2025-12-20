using System.Drawing;

namespace Hyphen.Sdk;

public class HyphenSdkColorExtensionsTests
{
	[Theory]
	[InlineData(255, 0, 0, "#ff0000")]
	[InlineData(0, 255, 0, "#00ff00")]
	[InlineData(0, 0, 255, "#0000ff")]
	public void ToCSS_RGB(byte red, byte green, byte blue, string expected)
	{
		var color = Color.FromArgb(red, green, blue);

		var result = color.ToCss();

		Assert.Equal(expected, result);
	}

	[Theory]
	[InlineData(255, 0, 0, 255, "#ff0000")]
	[InlineData(0, 255, 0, 255, "#00ff00")]
	[InlineData(0, 0, 255, 255, "#0000ff")]
	[InlineData(255, 0, 0, 127, "#ff00007f")]
	[InlineData(0, 255, 0, 127, "#00ff007f")]
	[InlineData(0, 0, 255, 127, "#0000ff7f")]
	public void RGBA(byte red, byte green, byte blue, byte alpha, string expected)
	{
		var color = Color.FromArgb(alpha, red, green, blue);

		var result = color.ToCss();

		Assert.Equal(expected, result);
	}
}
