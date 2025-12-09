using System.Diagnostics.CodeAnalysis;

namespace System.Net.Http;

[SuppressMessage("Design", "CA1034:Nested types should not be visible", Justification = "Extension blocks are implemented as nested types")]
internal static class HyphenSdkHttpClientExtensions
{
	extension(HttpClient client)
	{
		public void SetHyphenApiKey(string apiKey) =>
			client.DefaultRequestHeaders.Add("x-api-key", apiKey);
	}
}
