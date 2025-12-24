using System.Drawing;
using System.Globalization;

namespace Hyphen.Sdk;

internal static class HyphenSdkPolyfills
{
	extension(Color color)
	{
		public string ToCss()
		{
			var result = "#"
				+ ToHexChar(color.R >> 4) + ToHexChar(color.R & 0xF)
				+ ToHexChar(color.G >> 4) + ToHexChar(color.G & 0xF)
				+ ToHexChar(color.B >> 4) + ToHexChar(color.B & 0xF);

			if (color.A != 255)
				result = result + ToHexChar(color.A >> 4) + ToHexChar(color.A & 0xF);

			return result;
		}
	}

	extension(DateTimeOffset dateTimeOffset)
	{
		public string ToISO8601() =>
			dateTimeOffset.ToString(@"yyyy-MM-ddTHH\:mm\:ss.fff\Z", CultureInfo.InvariantCulture);
	}

	extension(File)
	{
#if NETSTANDARD
		public static Task WriteAllBytesAsync(string path, byte[] bytes, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			File.WriteAllBytes(path, bytes);
			return Task.CompletedTask;
		}
#endif
	}

	extension(HttpClient client)
	{
		public void SetHyphenApiKey(string apiKey) =>
			client.DefaultRequestHeaders.Add("x-api-key", apiKey);
	}

	extension(QrCodeSize size)
	{
		public string ToFormString() =>
			size switch
			{
				QrCodeSize.Small => "small",
				QrCodeSize.Medium => "medium",
				QrCodeSize.Large => "large",
				_ => throw new ArgumentException("Unknown QrCodeSize value: " + size.ToString()),
			};
	}

	extension(string str)
	{
#if NETSTANDARD
		public bool EndsWith(char value) => str.Length != 0 && str[str.Length - 1] == value;
#endif

		public string ReplaceInvariant(string oldValue, string newValue)
#if NETSTANDARD
			=> str.Replace(oldValue, newValue);
#else
			=> str.Replace(oldValue, newValue, StringComparison.InvariantCulture);
#endif

#if NETSTANDARD
		public bool StartsWith(char value) => str.Length != 0 && str[0] == value;
#endif
	}

	extension(string? str)
	{
		public string Quoted => str is null ? "null" : $@"""{str}""";
	}

	static char ToHexChar(int b) => (char)(b < 10 ? b + '0' : b - 10 + 'a');
}
