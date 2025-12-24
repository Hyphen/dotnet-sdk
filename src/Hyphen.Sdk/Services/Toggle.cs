using System.Collections.Concurrent;
using System.Net.Http.Json;
using System.Runtime.Caching;
using Hyphen.Sdk.Resources;

namespace Hyphen.Sdk;

internal class Toggle : BaseService, IToggle, IDisposable
{
	readonly ConcurrentDictionary<string, CacheItemPolicy?> cachePoliciesByToggleKey = [];
	readonly CacheKeyFactory cacheKeyFactory;
	readonly CachePolicyFactory cachePolicyFactory;
	readonly IHttpClientFactory httpClientFactory;

	public Toggle(IHttpClientFactory httpClientFactory, ILogger<IToggle> logger, IOptions<ToggleOptions> options)
		: base(logger)
	{
		this.httpClientFactory = Guard.ArgumentNotNull(httpClientFactory);
		Guard.ArgumentNotNull(Guard.ArgumentNotNull(options).Value, nameof(options));

		cacheKeyFactory = options.Value.CacheKeyFactory ?? DefaultCacheFactories.KeyFactory;
		cachePolicyFactory = options.Value.CachePolicyFactory ?? DefaultCacheFactories.PolicyFactory;
		DefaultContext = options.Value.DefaultContext;
		Environment = options.Value.Environment ?? "development";
		PublicKey = new ProjectPublicKey(options.Value.ProjectPublicKey);

		ApplicationId =
			options.Value.ApplicationId
				?? System.Environment.GetEnvironmentVariable(Env.AppId)
					?? throw new ArgumentException(HyphenSdkResources.Toggle_ApplicationIdRequired);

		DefaultTargetingKey =
			options.Value.DefaultTargetingKey
				?? $"{ApplicationId}-{Environment}-{Guid.NewGuid():n}";

		BaseUris =
			options.Value.BaseUris?.Count > 0
				? NormalizeUris(options.Value.BaseUris)
				: Env.IsDevEnvironment
					? [new("https://dev-horizon.hyphen.ai/")]
					: [new($"https://{PublicKey.OrganizationId}.toggle.hyphen.cloud/"), new("https://toggle.hyphen.cloud/")];
	}

	internal string ApplicationId { get; private set; }

	internal IReadOnlyCollection<Uri> BaseUris { get; private set; }

	internal MemoryCache Cache { get; } = new("IToggle");

	internal ToggleContext? DefaultContext { get; private set; }

	internal string DefaultTargetingKey { get; private set; }

	internal string Environment { get; private set; }

	internal ProjectPublicKey PublicKey { get; private set; }

	public void Dispose() => Cache.Dispose();

	public async Task<ToggleEvaluation<T?>> Evaluate<T>(string toggleKey, T? defaultValue, EvaluateParams? parms, CancellationToken cancellationToken)
	{
		var context = parms?.Context ?? DefaultContext;
		var cachePolicy = default(CacheItemPolicy);
		var cacheKey = default(string);

		if (parms?.Cache != false)
		{
			cachePolicy = cachePoliciesByToggleKey.GetOrAdd(toggleKey, tk => cachePolicyFactory(tk));
			if (cachePolicy is not null)
			{
				cacheKey = cacheKeyFactory(toggleKey, context);
				if (cacheKey is not null)
				{
					var evaluation = Cache.Get(cacheKey) as ToggleEvaluation<T?>;
					if (evaluation is not null)
						return evaluation;
				}
			}
		}

		var result = await EvaluateNoCache(context, toggleKey, defaultValue, cancellationToken).ConfigureAwait(false);

		if (cachePolicy is not null && cacheKey is not null)
			Cache.Add(cacheKey, result, cachePolicy);

		return result;
	}

	async Task<ToggleEvaluation<T?>> EvaluateNoCache<T>(ToggleContext? context, string toggleKey, T? defaultValue, CancellationToken cancellationToken)
	{
		var evaluationContext = new ToggleEvaluationContext
		{
			Application = ApplicationId,
			CustomAttributes = context?.CustomAttributes,
			Environment = Environment,
			IPAddress = context?.IPAddress,
			TargetingKey = context?.TargetingKey ?? context?.User?.Id ?? DefaultTargetingKey,
			User = context?.User,
		};

		var httpClient = httpClientFactory.CreateClient("IToggle");
		httpClient.SetHyphenApiKey(PublicKey);

		foreach (var baseUri in BaseUris)
		{
			var uri = new Uri(baseUri, "toggle/evaluate");

			try
			{
				var response = await httpClient.PostAsJsonAsync(uri, evaluationContext, cancellationToken: cancellationToken).ConfigureAwait(false);
				if (!response.IsSuccessStatusCode)
				{
					Logger.LogInformation("Request to {Uri} resulted in HTTP status code {StatusCode}", uri.ToString(), (int)response.StatusCode);
					continue;
				}

#if NETSTANDARD
				var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
#else
				var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
#endif

				var body = await JsonSerializer.DeserializeAsync<ToggleEvaluationResponse200>(stream, cancellationToken: cancellationToken).ConfigureAwait(false);
				if (body is null || body.Toggles is null)
				{
					Logger.LogInformation("Request to {Uri} resulted in an unparseable HTTP response", uri.ToString());
					continue;
				}

				if (!body.Toggles.TryGetValue(toggleKey, out var toggle) || toggle.Value is null)
				{
					Logger.LogInformation("Request to {Uri} returned a toggle set that did not include toggle key '{ToggleKey}'", uri.ToString(), toggleKey);
					continue;
				}

				// If the returned type is object and they're asking for it in any type other than string, we
				// grab the string value and deserialize that (since a string won't directly deserialize into
				// an object). Although we don't anticipate this usage, we also let them ask for objects as
				// strings in case the object may take various shapes based on the evaluation, and they want
				// to try to deserialize each in turn to see what actual shape they got back.
				var value =
					toggle.Type == ToggleEvaluationResponseItemType.Object
						&& toggle.Value.Value.ValueKind == JsonValueKind.String
						&& typeof(T) != typeof(string)
							? JsonSerializer.Deserialize<T>(toggle.Value.Value.GetString()!)
							: toggle.Value.Value.Deserialize<T>();

				return new() { Key = toggleKey, Reason = toggle.Reason, Value = value };
			}
			catch (TaskCanceledException)
			{
				return new()
				{
					Key = toggleKey,
					Reason = "Cancellation token was cancelled",
					Value = defaultValue,
				};
			}
			catch (JsonException)
			{
				Logger.LogInformation("Request to {Uri} resulted in an unparseable HTTP response", uri.ToString());
			}
			catch (Exception ex)
			{
				return new()
				{
					Exception = ex,
					Key = toggleKey,
					Reason = $"Request to {uri} resulted in an exception",
					Value = defaultValue,
				};
			}
		}

		return new()
		{
			Key = toggleKey,
			Reason = "No Toggle servers were available for the request",
			Value = defaultValue,
		};
	}

	static IReadOnlyCollection<Uri> NormalizeUris(IEnumerable<Uri> uris) =>
		[.. uris.Select(uri => uri.ToString().EndsWith('/') ? uri : new Uri(uri + "/"))];
}
