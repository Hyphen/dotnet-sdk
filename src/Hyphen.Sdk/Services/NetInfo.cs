using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Hyphen.Sdk.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Hyphen.Sdk;

internal class NetInfo(IHttpClientFactory httpClientFactory, ILogger<INetInfo> logger, IOptions<NetInfoOptions> options)
	: BaseService(httpClientFactory, logger, options), INetInfo
{
	protected Uri BaseUri { get; } = Guard.ArgumentNotNull(options).Value.BaseUri;

	public async ValueTask<NetInfoResult[]> GetIPInfos(string[] ips, CancellationToken cancellationToken)
	{
		var uri = new Uri(BaseUri, "ip");
		var client = HttpClientFactory.CreateClient(nameof(INetInfo));
		client.SetHyphenApiKey(ApiKey);

		try
		{
			if (Guard.ArgumentNotNull(ips).Length == 0)
			{
				var result = await client.GetFromJsonAsync<NetInfoResult>(uri, cancellationToken).ConfigureAwait(false);
				if (result is null)
					return WithError(ips, "Could not deserialize response");

				return [result];
			}
			else
			{
				var response = await client.PostAsJsonAsync(uri, ips, cancellationToken).ConfigureAwait(false);
				if (!response.IsSuccessStatusCode)
					return WithError(ips, $"HTTP response code {(int)response.StatusCode} ({response.StatusCode})");

#if NET
				var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
#else
				var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
#endif

				var result = await JsonSerializer.DeserializeAsync<NetInfoPostResponse200>(stream, cancellationToken: cancellationToken).ConfigureAwait(false);
				if (result is null)
					return WithError(ips, "Could not deserialize response");

				return result.Data;
			}
		}
		catch (Exception ex)
		{
			return WithError(ips, ex.Message);
		}
	}

	static NetInfoResult[] WithError(string[] ips, string errorMessage) =>
		[.. ips.Select(ip => new NetInfoResult { IP = ip, Type = IPType.Error, ErrorMessage = errorMessage })];

	class NetInfoPostResponse200
	{
		[JsonPropertyName("data")]
		public required NetInfoResult[] Data { get; set; }
	}
}
