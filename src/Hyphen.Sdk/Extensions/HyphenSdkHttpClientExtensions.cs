namespace System.Net.Http;

internal static class HyphenSdkHttpClientExtensions
{
	public static void SetHyphenApiKey(this HttpClient client, string apiKey) =>
		client.DefaultRequestHeaders.Add("x-api-key", apiKey);
}
