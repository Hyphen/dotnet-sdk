using System.Net.Http.Json;
using Hyphen.Sdk.Internal;
using Hyphen.Sdk.Resources;

namespace Hyphen.Sdk;

internal class NetInfo(IHttpClientFactory httpClientFactory, ILogger<INetInfo> logger, IOptions<NetInfoOptions> options)
	: BaseHttpService(httpClientFactory, logger, options), INetInfo
{
	internal Uri BaseUri { get; } =
		Guard.ArgumentNotNull(options).Value.BaseUri
			?? (Env.IsDevEnvironment ? new("https://dev.net.info") : new("https://net.info"));

	public Task<NetInfoResult[]> GetIPInfos(string[] ips, CancellationToken cancellationToken)
	{
		Guard.ArgumentNotNull(ips);

		var uri = new Uri(BaseUri, "ip");
		var client = HttpClientFactory.CreateClient(nameof(INetInfo));
		client.SetHyphenApiKey(ApiKey);

		if (ips.Length == 0)
			return ProcessResponse<NetInfoResult>(
				["unknown"],
				() => client.GetAsync(uri, cancellationToken),
				content => [content],
				cancellationToken
			);
		else
			return ProcessResponse<NetInfoPostResponse200>(
				ips,
				() => client.PostAsJsonAsync(uri, ips, cancellationToken),
				content => content.Data,
				cancellationToken
			);
	}

	async static Task<NetInfoResult[]> ProcessResponse<T>(
		string[] ips,
		Func<Task<HttpResponseMessage>> requestThunk,
		Func<T, NetInfoResult[]> contentProcessor,
		CancellationToken cancellationToken)
	{
		try
		{
			var response = await requestThunk().ConfigureAwait(false);
			if (!response.IsSuccessStatusCode)
				return WithError(ips, HyphenSdkResources.Http_StatusCodeError(response.StatusCode));

#if NETSTANDARD
			var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
#else
			var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
#endif

			var body = await JsonSerializer.DeserializeAsync<T>(stream, cancellationToken: cancellationToken).ConfigureAwait(false);
			if (body is null)
				return WithError(ips, HyphenSdkResources.Http_ResponseMalformed);

			var result = contentProcessor(body);

			for (var idx = 0; idx < result.Length; ++idx)
				if (result[idx] is null)
					result[idx] = new NetInfoResult
					{
						IP = idx >= ips.Length ? "unknown" : ips[idx],
						Type = IPType.Error,
						ErrorMessage = HyphenSdkResources.Http_ResponseMalformed
					};

			return result;
		}
		catch (JsonException)
		{
			return WithError(ips, HyphenSdkResources.Http_ResponseMalformed);
		}
		catch (Exception ex)
		{
			return WithError(ips, $"{ex.GetType().FullName}: {ex.Message}");
		}
	}

	static NetInfoResult[] WithError(string[] ips, string errorMessage) =>
		[.. ips.Select(ip => new NetInfoResult { IP = ip, Type = IPType.Error, ErrorMessage = errorMessage })];
}
