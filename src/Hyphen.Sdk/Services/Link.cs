using System.Drawing;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Web;
using Hyphen.Sdk.Resources;

namespace Hyphen.Sdk;

internal class Link(IHttpClientFactory httpClientFactory, ILogger<ILink> logger, IOptions<LinkOptions> options)
	: BaseHttpService(httpClientFactory, logger, options), ILink
{
	static readonly JsonSerializerOptions jsonSerializerOptions = new() { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };

	internal Uri BaseUri { get; } = GetBaseUri(Guard.ArgumentNotNull(options).Value.BaseUriTemplate, options.Value.OrganizationId);

	public async Task<QrCodeResult> CreateQrCode(string shortCodeId, CreateQrCodeParams? parms, CancellationToken cancellationToken)
	{
		Guard.ArgumentNotNull(shortCodeId);

		var uri = new Uri(BaseUri, $"{HttpUtility.UrlEncode(shortCodeId)}/qrs");
		var client = HttpClientFactory.CreateClient(nameof(ILink));
		client.SetHyphenApiKey(ApiKey);

		using var form = new MultipartFormDataContent("hyphen-dotnet-sdk");
#pragma warning disable CA2000 // The child content objects are disposed by MultipartFormDataContent
		if (parms?.BackgroundColor is not null)
			form.Add(new StringContent(parms.BackgroundColor.Value.ToCss()), "backgroundColor");
		if (parms?.Color is not null)
			form.Add(new StringContent(parms.Color.Value.ToCss()), "color");
		if (parms?.Logo is not null)
		{
			var logoContent = new ByteArrayContent(parms.Logo);
			logoContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");
			form.Add(logoContent, "logo");
		}
		if (parms?.Size is not null)
			form.Add(new StringContent(parms.Size.Value.ToFormString()), "size");
		if (parms?.Title is not null)
			form.Add(new StringContent(parms.Title), "title");
#pragma warning restore CA2000

		var response = await client.PostAsync(uri, form.Any() ? form : null, cancellationToken).ConfigureAwait(false);
		return
			await ProcessResponse<QrCodeResult>(response, cancellationToken).ConfigureAwait(false)
				?? throw new NotFoundException(HyphenSdkResources.Link_CreateQrCode404);
	}

	public async Task<ShortCodeResult> CreateShortCode(Uri longUrl, string domain, CreateShortCodeParams? parms, CancellationToken cancellationToken)
	{
		Guard.ArgumentNotNull(longUrl);
		Guard.ArgumentNotNull(domain);

		var client = HttpClientFactory.CreateClient(nameof(ILink));
		client.SetHyphenApiKey(ApiKey);

		var postBody = new
		{
			long_url = longUrl,
			domain,
			code = parms?.Code,
			title = parms?.Title,
			tags = parms?.Tags,
		};

		var response = await client.PostAsJsonAsync(BaseUri, postBody, jsonSerializerOptions, cancellationToken).ConfigureAwait(false);
		return
			await ProcessResponse<ShortCodeResult>(response, cancellationToken).ConfigureAwait(false)
				?? throw new NotFoundException(HyphenSdkResources.Link_CreateShortCode404);
	}

	public async Task DeleteQrCode(string shortCodeId, string qrCodeId, CancellationToken cancellationToken)
	{
		Guard.ArgumentNotNull(shortCodeId);
		Guard.ArgumentNotNull(qrCodeId);

		var uri = new Uri(BaseUri, $"{HttpUtility.UrlEncode(shortCodeId)}/qrs/{HttpUtility.UrlEncode(qrCodeId)}");
		var client = HttpClientFactory.CreateClient(nameof(ILink));
		client.SetHyphenApiKey(ApiKey);

		var response = await client.DeleteAsync(uri, cancellationToken).ConfigureAwait(false);
		await ProcessResponse(response, cancellationToken).ConfigureAwait(false);
	}

	public async Task DeleteShortCode(string shortCodeId, CancellationToken cancellationToken)
	{
		Guard.ArgumentNotNull(shortCodeId);

		var uri = new Uri(BaseUri, HttpUtility.UrlEncode(shortCodeId));
		var client = HttpClientFactory.CreateClient(nameof(ILink));
		client.SetHyphenApiKey(ApiKey);

		var response = await client.DeleteAsync(uri, cancellationToken).ConfigureAwait(false);
		await ProcessResponse(response, cancellationToken).ConfigureAwait(false);
	}

	public async Task<QrCodeResult?> GetQrCode(string shortCodeId, string qrCodeId, CancellationToken cancellationToken)
	{
		Guard.ArgumentNotNull(shortCodeId);
		Guard.ArgumentNotNull(qrCodeId);

		var uri = new Uri(BaseUri, $"{HttpUtility.UrlEncode(shortCodeId)}/qrs/{HttpUtility.UrlEncode(qrCodeId)}");
		var client = HttpClientFactory.CreateClient(nameof(ILink));
		client.SetHyphenApiKey(ApiKey);

		var response = await client.GetAsync(uri, cancellationToken).ConfigureAwait(false);
		return await ProcessResponse<QrCodeResult>(response, cancellationToken).ConfigureAwait(false);
	}

	public async Task<PagedResult<QrCodeResult>?> GetQrCodes(string shortCodeId, GetQrCodesParams? parms, CancellationToken cancellationToken)
	{
		Guard.ArgumentNotNull(shortCodeId);

		var query = new QueryStringBuilder();
		query.Add("pageNum", parms?.PageNumber);
		query.Add("pageSize", parms?.PageSize);

		var uri = new Uri(BaseUri, $"{HttpUtility.UrlEncode(shortCodeId)}/qrs{query}");
		var client = HttpClientFactory.CreateClient(nameof(ILink));
		client.SetHyphenApiKey(ApiKey);

		var response = await client.GetAsync(uri, cancellationToken).ConfigureAwait(false);
		return await ProcessResponse<PagedResult<QrCodeResult>>(response, cancellationToken).ConfigureAwait(false);
	}

	public async Task<ShortCodeResult?> GetShortCode(string shortCodeId, CancellationToken cancellationToken)
	{
		Guard.ArgumentNotNull(shortCodeId);

		var uri = new Uri(BaseUri, HttpUtility.UrlEncode(shortCodeId));
		var client = HttpClientFactory.CreateClient(nameof(ILink));
		client.SetHyphenApiKey(ApiKey);

		var response = await client.GetAsync(uri, cancellationToken).ConfigureAwait(false);
		return await ProcessResponse<ShortCodeResult>(response, cancellationToken).ConfigureAwait(false);
	}

	public async Task<PagedResult<ShortCodeResult>?> GetShortCodes(GetShortCodesParams? parms, CancellationToken cancellationToken)
	{
		var query = new QueryStringBuilder();
		query.Add("pageNum", parms?.PageNumber);
		query.Add("pageSize", parms?.PageSize);
		query.Add("search", parms?.Search);
		if (parms?.Tags?.Count > 0)
			query.Add("tags", string.Join(",", parms.Tags));

		var uri = new Uri(BaseUri, query.ToString());
		var client = HttpClientFactory.CreateClient(nameof(ILink));
		client.SetHyphenApiKey(ApiKey);

		var response = await client.GetAsync(uri, cancellationToken).ConfigureAwait(false);
		return await ProcessResponse<PagedResult<ShortCodeResult>>(response, cancellationToken).ConfigureAwait(false);
	}

	public async Task<ShortCodeStatsResult?> GetShortCodeStats(string shortCodeId, DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken)
	{
		Guard.ArgumentNotNull(shortCodeId);

		var uri = new Uri(BaseUri, $"{HttpUtility.UrlEncode(shortCodeId)}/stats?startDate={startDate.ToISO8601()}&endDate={endDate.ToISO8601()}");
		var client = HttpClientFactory.CreateClient(nameof(ILink));
		client.SetHyphenApiKey(ApiKey);

		var response = await client.GetAsync(uri, cancellationToken).ConfigureAwait(false);
		return await ProcessResponse<ShortCodeStatsResult>(response, cancellationToken).ConfigureAwait(false);
	}

	public async Task<string[]?> GetTags(CancellationToken cancellationToken)
	{
		var uri = new Uri(BaseUri, "tags");
		var client = HttpClientFactory.CreateClient(nameof(ILink));
		client.SetHyphenApiKey(ApiKey);

		var response = await client.GetAsync(uri, cancellationToken).ConfigureAwait(false);
		return await ProcessResponse<string[]>(response, cancellationToken).ConfigureAwait(false);
	}

	public async Task<ShortCodeResult> UpdateShortCode(string shortCodeId, UpdateShortCodeParams parms, CancellationToken cancellationToken)
	{
		Guard.ArgumentNotNull(shortCodeId);
		Guard.ArgumentNotNull(parms);

		var uri = new Uri(BaseUri, HttpUtility.UrlEncode(shortCodeId));
		var client = HttpClientFactory.CreateClient(nameof(ILink));
		client.SetHyphenApiKey(ApiKey);

		var patchBody = new
		{
			long_url = parms.LongUrl,
			title = parms.Title,
			tags = parms.Tags,
		};

		var response = await client.PatchAsJsonAsync(uri, patchBody, jsonSerializerOptions, cancellationToken).ConfigureAwait(false);
		return
			await ProcessResponse<ShortCodeResult>(response, cancellationToken).ConfigureAwait(false)
				?? throw new NotFoundException(HyphenSdkResources.Link_UpdateShortCode404);
	}

	// Helpers

	static Uri GetBaseUri(string? baseUriTemplate, OrganizationId organizationId)
	{
		baseUriTemplate ??=
			Env.IsDevEnvironment
				? "https://dev-api.hyphen.ai/api/organizations/{organizationId}/link/codes"
				: "https://api.hyphen.ai/api/organizations/{organizationId}/link/codes";

		return new(baseUriTemplate.ReplaceInvariant("{organizationId}", organizationId).TrimEnd('/') + '/');
	}

	static async Task<bool> ProcessResponse(HttpResponseMessage response, CancellationToken cancellationToken)
	{
		if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
			throw new ApiKeyException(HyphenSdkResources.Http_InvalidApiKey);
		if (response.StatusCode == HttpStatusCode.NotFound || response.StatusCode == HttpStatusCode.NoContent)
			return false;
		if (!response.IsSuccessStatusCode)
			throw new HttpStatusCodeException(response.StatusCode);

		return true;
	}

	static async Task<T?> ProcessResponse<T>(HttpResponseMessage response, CancellationToken cancellationToken)
		where T : notnull
	{
		if (!await ProcessResponse(response, cancellationToken).ConfigureAwait(false))
			return default;

		try
		{
#if NETSTANDARD
			using var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
#else
			using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
#endif
			return
				await JsonSerializer.DeserializeAsync<T>(stream, cancellationToken: cancellationToken).ConfigureAwait(false)
					?? throw new HttpStatusCodeException(HttpStatusCode.ServiceUnavailable, HyphenSdkResources.Http_ResponseMalformed);
		}
		catch (JsonException)
		{
			throw new HttpStatusCodeException(HttpStatusCode.ServiceUnavailable, HyphenSdkResources.Http_ResponseMalformed);
		}
	}
}
