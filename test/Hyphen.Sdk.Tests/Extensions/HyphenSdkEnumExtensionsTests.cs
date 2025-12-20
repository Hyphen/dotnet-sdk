namespace Hyphen.Sdk;

public class HyphenSdkEnumExtensionsTests
{
	[Theory]
	[InlineData(QrCodeSize.Large, "large")]
	[InlineData(QrCodeSize.Medium, "medium")]
	[InlineData(QrCodeSize.Small, "small")]
	public void SuccessCases(QrCodeSize qrCodeSize, string expected)
	{
		var result = qrCodeSize.ToFormString();

		Assert.Equal(expected, result);
	}

	[Fact]
	public void GuardClause()
	{
		var ex = Record.Exception(() => ((QrCodeSize)99).ToFormString());

		Assert.IsType<ArgumentException>(ex);
		Assert.Equal("Unknown QrCodeSize value: 99", ex.Message);
	}
}
