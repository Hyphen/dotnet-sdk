using System.Text;
using Hyphen.Sdk.Resources;

namespace Hyphen.Sdk;

public class QrCodeResultTests
{
	public class GetQrCodeBytes
	{
		[Fact]
		public void UnknownHeader()
		{
			var qcr = new QrCodeResult { Id = "id", Link = new("http://localhost"), QrCode = "unknown" };

			var ex = Record.Exception(qcr.GetQrCodeBytes);

			Assert.IsType<InvalidOperationException>(ex);
			Assert.Equal(HyphenSdkResources.Link_QrCodeInvalidFormat, ex.Message);
		}

		[Fact]
		public void InvalidBase64Value()
		{
			var qcr = new QrCodeResult { Id = "id", Link = new("http://localhost"), QrCode = "data:image/png;base64,Hello World *$" };

			var ex = Record.Exception(qcr.GetQrCodeBytes);

			Assert.IsType<FormatException>(ex);
		}

		[Fact]
		public void ValidBase64Value()
		{
			var helloWorldBytes = Encoding.UTF8.GetBytes("Hello, world");
			var helloWorldBase64 = Convert.ToBase64String(helloWorldBytes);
			var qcr = new QrCodeResult { Id = "id", Link = new("http://localhost"), QrCode = "data:image/png;base64," + helloWorldBase64 };

			var result = qcr.GetQrCodeBytes();

			Assert.Equal(helloWorldBytes, result);
		}
	}
}
