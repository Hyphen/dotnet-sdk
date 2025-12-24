using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Hyphen.Sdk.Resources;

namespace Hyphen.Sdk;

[CleanEnvironment(Env.AppId, Env.Dev, Env.ProjectPublicKey)]
public class ToggleTests
{
	// This public key contains org_123456789012345678901234 and proj_123456789012345678901234
	const string DefaultProjectPublicKey = "public_b3JnXzEyMzQ1Njc4OTAxMjM0NTY3ODkwMTIzNDpwcm9qXzEyMzQ1Njc4OTAxMjM0NTY3ODkwMTIzNDppbnZhbGlkLWtleQ==";

	readonly IHttpClientFactory httpClientFactory;
	readonly SpyLogger<IToggle> logger;
	readonly ToggleOptions options = new() { ApplicationId = "abc123", ProjectPublicKey = DefaultProjectPublicKey };

	public ToggleTests()
	{
		httpClientFactory = Substitute.For<IHttpClientFactory>();
		logger = new SpyLogger<IToggle>();
	}

	public class Constructor : ToggleTests
	{
		[Fact]
		public void GuardClauses()
		{
			Assert.Throws<ArgumentNullException>("httpClientFactory", () => new Toggle(null!, logger, Options.Create(options)));
			Assert.Throws<ArgumentNullException>("logger", () => new Toggle(httpClientFactory, null!, Options.Create(options)));
			Assert.Throws<ArgumentNullException>("options", () => new Toggle(httpClientFactory, logger, null!));
		}

		[Fact]
		public void GetsApplicationId()
		{
			options.ApplicationId = null;

			var ex = Assert.Throws<ArgumentException>(() => new Toggle(httpClientFactory, logger, Options.Create(options)));
			Assert.Equal(HyphenSdkResources.Toggle_ApplicationIdRequired, ex.Message);

			// Override with environment variable
			Environment.SetEnvironmentVariable(Env.AppId, "def456");

			using var toggle = new Toggle(httpClientFactory, logger, Options.Create(options));

			Assert.Equal("def456", toggle.ApplicationId);
		}

		[Fact]
		public void GetsPublicKey()
		{
			options.ProjectPublicKey = "abc123";

			var ex1 = Assert.Throws<PublicKeyException>(() => new Toggle(httpClientFactory, logger, Options.Create(options)));
			Assert.Equal(HyphenSdkResources.ProjectPublicKey_MustBePublic, ex1.Message);

			options.ProjectPublicKey = "public_";  // Missing data in the encoded part of the key

			var ex2 = Assert.Throws<PublicKeyException>(() => new Toggle(httpClientFactory, logger, Options.Create(options)));
			Assert.Equal(HyphenSdkResources.ProjectPublicKey_Malformed, ex2.Message);

			options.ProjectPublicKey = "public_abc123";  // Not base64 encoded

			var ex3 = Assert.Throws<PublicKeyException>(() => new Toggle(httpClientFactory, logger, Options.Create(options)));
			Assert.Equal(HyphenSdkResources.ProjectPublicKey_Malformed, ex3.Message);

			options.ProjectPublicKey = null;

			var ex4 = Assert.Throws<PublicKeyException>(() => new Toggle(httpClientFactory, logger, Options.Create(options)));
			Assert.Equal(HyphenSdkResources.ProjectPublicKey_Required, ex4.Message);

			// Override with environment variable
			Environment.SetEnvironmentVariable(Env.ProjectPublicKey, DefaultProjectPublicKey);

			using var toggle = new Toggle(httpClientFactory, logger, Options.Create(options));

			Assert.Equal(DefaultProjectPublicKey, toggle.PublicKey);
			Assert.Equal("org_123456789012345678901234", toggle.PublicKey.OrganizationId);
			Assert.Equal("proj_123456789012345678901234", toggle.PublicKey.ProjectId);
		}

		[Theory]
		[InlineData(null, "development")]
		[InlineData("foo", "foo")]
		public void GetsEnvironment(string? optionValue, string expected)
		{
			options.Environment = optionValue;

			using var toggle = new Toggle(httpClientFactory, logger, Options.Create(options));

			Assert.Equal(expected, toggle.Environment);
		}

		[Theory]
		[InlineData(null, "abc123-development-")]
		[InlineData("def456", "def456")]
		public void GetsDefaultTargetingKey(string? optionValue, string expectedStartsWith)
		{
			options.DefaultTargetingKey = optionValue;

			using var toggle = new Toggle(httpClientFactory, logger, Options.Create(options));

			Assert.StartsWith(expectedStartsWith, toggle.DefaultTargetingKey);
		}

		[Theory]
		[InlineData("http://localhost:12345/foo", null, "http://localhost:12345/foo/")]  // Normalized with trailing slash
		[InlineData("http://localhost:54321/foo/", null, "http://localhost:54321/foo/")]
		[InlineData(null, null, "https://org_123456789012345678901234.toggle.hyphen.cloud/", "https://toggle.hyphen.cloud/")]
		[InlineData(null, "true", "https://dev-horizon.hyphen.ai/")]
		public void GetsBaseUris(string? baseUri, string? devEnvironmentValue, params string[] expectedUris)
		{
			options.BaseUris = baseUri is not null ? [new(baseUri)] : null;
			Environment.SetEnvironmentVariable(Env.Dev, devEnvironmentValue);

			using var toggle = new Toggle(httpClientFactory, logger, Options.Create(options));

			Assert.Equal(expectedUris, toggle.BaseUris.Select(uri => uri.ToString()));
		}
	}

	public class Caching : ToggleTests
	{
		readonly ToggleEvaluation<bool> cachedResult = new() { Key = "toggleKey", Reason = "cached", Value = true };

		public Caching() =>
			ReturnContent(new { toggles = new { toggleKey = new { key = "toggleKey", reason = "not cached", type = "boolean", value = true } } });

		[Theory]
		[InlineData(null, null, "toggleKey")]
		[InlineData("id", null, "toggleKey-id")]
		[InlineData(null, "email", "toggleKey-email")]
		[InlineData("id", "email", "toggleKey-id")]
		public async ValueTask PullsResultFromCacheWhenPresent(string? userId, string? userEmail, string expectedCacheKey)
		{
			using var toggle = new Toggle(httpClientFactory, logger, Options.Create(options));
			toggle.Cache.Add(expectedCacheKey, cachedResult, DateTimeOffset.MaxValue);
			var parms = new EvaluateParams { Context = new() { User = new() { Id = userId, Email = userEmail } } };

			var result = await toggle.Evaluate<bool>("toggleKey", parms, TestContext.Current.CancellationToken);

			Assert.Equal("cached", result.Reason);
		}

		[Fact]
		public async ValueTask SavesNewValueToCache()
		{
			using var toggle = new Toggle(httpClientFactory, logger, Options.Create(options));

			var result = await toggle.Evaluate<bool>("toggleKey", TestContext.Current.CancellationToken);

			Assert.Equal("not cached", result.Reason);
		}

		[Fact]
		public async ValueTask SkipsCacheWhenInstructedByParams()
		{
			using var toggle = new Toggle(httpClientFactory, logger, Options.Create(options));
			toggle.Cache.Add("toggleKey", cachedResult, DateTimeOffset.MaxValue);
			var parms = new EvaluateParams { Cache = false };

			var result = await toggle.Evaluate<bool>("toggleKey", parms, TestContext.Current.CancellationToken);

			Assert.Equal("not cached", result.Reason);
		}

		[Fact]
		public async ValueTask SkipsCacheWhenInstructedByPolicyFactory()
		{
			options.CachePolicyFactory = key => null;
			using var toggle = new Toggle(httpClientFactory, logger, Options.Create(options));
			toggle.Cache.Add("toggleKey", cachedResult, DateTimeOffset.MaxValue);

			var result = await toggle.Evaluate<bool>("toggleKey", TestContext.Current.CancellationToken);

			Assert.Equal("not cached", result.Reason);
		}

		[Fact]
		public async ValueTask SkipsCacheWhenInstructedByKeyFactory()
		{
			options.CacheKeyFactory = (key, context) => null;
			using var toggle = new Toggle(httpClientFactory, logger, Options.Create(options));
			toggle.Cache.Add("toggleKey", cachedResult, DateTimeOffset.MaxValue);

			var result = await toggle.Evaluate<bool>("toggleKey", TestContext.Current.CancellationToken);

			Assert.Equal("not cached", result.Reason);
		}
	}

	public class DataTypes : ToggleTests
	{
		[Theory]
		[InlineData(true, "boolean")]
		[InlineData(false, "boolean")]
		[InlineData(-1, "number")]
		[InlineData(42, "number")]
		[InlineData(42_000_000_000, "number")]
		[InlineData(21.12, "number")]
		[InlineData("Hello world", "string")]
		[InlineData(@"{""foo"": 42}", "object")]  // Allow objects to be retrieved as strings
		public async ValueTask GetIntrinsic<T>(T value, string type)
		{
			using var toggle = new Toggle(httpClientFactory, logger, Options.Create(options));
			ReturnContent(new { toggles = new { toggleKey = new { key = "toggleKey", reason = "reason", type, value } } });

			var result = await toggle.Evaluate<T>("toggleKey", TestContext.Current.CancellationToken);

			Assert.Equal("toggleKey", result.Key);
			Assert.Equal("reason", result.Reason);
			Assert.Equal(value, result.Value);
			Assert.Null(result.Exception);
			Assert.Empty(logger.Messages);
		}

		[Fact]
		public async ValueTask GetObject()
		{
			using var toggle = new Toggle(httpClientFactory, logger, Options.Create(options));
			ReturnContent(new { toggles = new { toggleKey = new { key = "toggleKey", reason = "reason", type = "object", value = @"{""foo"": 42}" } } });

			var result = await toggle.Evaluate<ComplexObject>("toggleKey", TestContext.Current.CancellationToken);

			Assert.Equal("toggleKey", result.Key);
			Assert.Equal("reason", result.Reason);
			Assert.Equal(42, result.Value!.Foo);
			Assert.Null(result.Exception);
			Assert.Empty(logger.Messages);
		}

		class ComplexObject
		{
			[JsonPropertyName("foo")]
			public int Foo { get; set; }
		}
	}

	public class RequestUriHandling : ToggleTests
	{
		[Fact]
		public async ValueTask ReturnsFirstResponse()
		{
			using var toggle = new Toggle(httpClientFactory, logger, Options.Create(options));
			ReturnContents(
				(
					"https://org_123456789012345678901234.toggle.hyphen.cloud/toggle/evaluate",
					new { toggles = new { toggleKey = new { key = "toggleKey", reason = "You're the first", type = "boolean", value = true } } }
				),
				(
					"https://toggle.hyphen.cloud/toggle/evaluate",
					new { toggles = new { toggleKey = new { key = "toggleKey", reason = "You're the second", type = "boolean", value = false } } }
				)
			);

			var result = await toggle.Evaluate<bool>("toggleKey", TestContext.Current.CancellationToken);

			Assert.Equal("toggleKey", result.Key);
			Assert.True(result.Value);
			Assert.Equal("You're the first", result.Reason);
			Assert.Null(result.Exception);
			Assert.Empty(logger.Messages);
		}

		[Fact]
		public async ValueTask ReturnsSecondResponse_FirstNotPresent()
		{
			using var toggle = new Toggle(httpClientFactory, logger, Options.Create(options));
			ReturnContents(
				(
					"https://toggle.hyphen.cloud/toggle/evaluate",
					new { toggles = new { toggleKey = new { key = "toggleKey", reason = "You're the second", type = "boolean", value = false } } }
				)
			);

			var result = await toggle.Evaluate<bool>("toggleKey", TestContext.Current.CancellationToken);

			Assert.False(result.Value);
			Assert.Equal("You're the second", result.Reason);
			var message = Assert.Single(logger.Messages);
			Assert.Equal("[Information] Request to https://org_123456789012345678901234.toggle.hyphen.cloud/toggle/evaluate resulted in HTTP status code 404", message);
		}

		[Fact]
		public async ValueTask ReturnsSecondResponse_FirstHasNoToggleData()
		{
			using var toggle = new Toggle(httpClientFactory, logger, Options.Create(options));
			ReturnContents(
				(
					"https://org_123456789012345678901234.toggle.hyphen.cloud/toggle/evaluate",
					new { toggles = new { } }
				),
				(
					"https://toggle.hyphen.cloud/toggle/evaluate",
					new { toggles = new { toggleKey = new { key = "toggleKey", reason = "You're the second", type = "boolean", value = false } } }
				)
			);

			var result = await toggle.Evaluate<bool>("toggleKey", TestContext.Current.CancellationToken);

			Assert.False(result.Value);
			Assert.Equal("You're the second", result.Reason);
			var message = Assert.Single(logger.Messages);
			Assert.Equal("[Information] Request to https://org_123456789012345678901234.toggle.hyphen.cloud/toggle/evaluate returned a toggle set that did not include toggle key 'toggleKey'", message);
		}

		[Fact]
		public async ValueTask ReturnsSecondResponse_FirstHasMalformedData()
		{
			using var toggle = new Toggle(httpClientFactory, logger, Options.Create(options));
			ReturnContents(
				(
					"https://org_123456789012345678901234.toggle.hyphen.cloud/toggle/evaluate",
					new { }
				),
				(
					"https://toggle.hyphen.cloud/toggle/evaluate",
					new { toggles = new { toggleKey = new { key = "toggleKey", reason = "You're the second", type = "boolean", value = false } } }
				)
			);

			var result = await toggle.Evaluate<bool>("toggleKey", TestContext.Current.CancellationToken);

			Assert.False(result.Value);
			Assert.Equal("You're the second", result.Reason);
			var message = Assert.Single(logger.Messages);
			Assert.Equal("[Information] Request to https://org_123456789012345678901234.toggle.hyphen.cloud/toggle/evaluate resulted in an unparseable HTTP response", message);
		}

		[Fact]
		public async ValueTask ReturnsSecondResponse_FirstHasNull()
		{
			using var toggle = new Toggle(httpClientFactory, logger, Options.Create(options));
			ReturnContents(
				(
					"https://org_123456789012345678901234.toggle.hyphen.cloud/toggle/evaluate",
					null
				),
				(
					"https://toggle.hyphen.cloud/toggle/evaluate",
					new { toggles = new { toggleKey = new { key = "toggleKey", reason = "You're the second", type = "boolean", value = false } } }
				)
			);

			var result = await toggle.Evaluate<bool>("toggleKey", TestContext.Current.CancellationToken);

			Assert.False(result.Value);
			Assert.Null(result.Exception);
			Assert.Equal<object>("You're the second", result.Reason);
			var message = Assert.Single(logger.Messages);
			Assert.Equal("[Information] Request to https://org_123456789012345678901234.toggle.hyphen.cloud/toggle/evaluate resulted in an unparseable HTTP response", message);
		}

		[Theory]
		[InlineData(true)]
		[InlineData(false)]
		public async ValueTask NoResponses_ReturnsDefaultValue(bool defaultValue)
		{
			using var toggle = new Toggle(httpClientFactory, logger, Options.Create(options));
			ReturnContents();

			var result = await toggle.Evaluate("toggleKey", defaultValue, TestContext.Current.CancellationToken);

			Assert.Equal(defaultValue, result.Value);
			Assert.Equal("No Toggle servers were available for the request", result.Reason);
		}

		[Theory]
		[InlineData(true)]
		[InlineData(false)]
		public async ValueTask CancelledTask(bool defaultValue)
		{
			var cts = new CancellationTokenSource();
			cts.Cancel();
			using var toggle = new Toggle(httpClientFactory, logger, Options.Create(options));
			ReturnContents(
				(
					"https://org_123456789012345678901234.toggle.hyphen.cloud/toggle/evaluate",
					new { toggles = new { toggleKey = new { key = "toggleKey", reason = "You're the first", type = "boolean", value = true } } }
				),
				(
					"https://toggle.hyphen.cloud/toggle/evaluate",
					new { toggles = new { toggleKey = new { key = "toggleKey", reason = "You're the second", type = "boolean", value = false } } }
				)
			);

			var result = await toggle.Evaluate<bool>("toggleKey", defaultValue, cts.Token);

			Assert.Equal(defaultValue, result.Value);
			Assert.Equal("Cancellation token was cancelled", result.Reason);
		}

		[Theory]
		[InlineData(true)]
		[InlineData(false)]
		public async ValueTask ExceptionThrown_ReturnsDefaultValue(bool defaultValue)
		{
			using var toggle = new Toggle(httpClientFactory, logger, Options.Create(options));
			Throws(new DivideByZeroException());

			var result = await toggle.Evaluate("toggleKey", defaultValue, TestContext.Current.CancellationToken);

			Assert.Equal(defaultValue, result.Value);
			Assert.Equal("Request to https://org_123456789012345678901234.toggle.hyphen.cloud/toggle/evaluate resulted in an exception", result.Reason);
			Assert.IsType<DivideByZeroException>(result.Exception);
		}

	}

	public class TargetingKey : ToggleTests
	{
		[Theory]
		[InlineData(null, null, "def456")]
		[InlineData("ghi789", null, "ghi789")]
		[InlineData(null, "user123", "user123")]
		[InlineData("ghi789", "user123", "ghi789")]
		public async ValueTask TargetingKeyFallback(string? targetingKeyOverride, string? userId, string expectedTargetingKey)
		{
			options.BaseUris = [new("http://localhost:12345/")];
			options.DefaultTargetingKey = "def456";
			var parms = new EvaluateParams { Context = new ToggleContext { TargetingKey = targetingKeyOverride, User = new() { Id = userId } } };
			using var toggle = new Toggle(httpClientFactory, logger, Options.Create(options));
			var handler = ReturnContents();

			await toggle.Evaluate<bool>("toggleKey", parms, TestContext.Current.CancellationToken);

			var request = Assert.Single(handler.Requests);
			Assert.Equal("http://localhost:12345/toggle/evaluate", request.Uri);
			var requestContext = JsonSerializer.Deserialize<ToggleEvaluationContext>(request.Body!);
			Assert.Equal(expectedTargetingKey, requestContext!.TargetingKey);
		}
	}

	SpyHttpMessageHandler ReturnContent(dynamic content)
	{
		var handler = new SpyHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK) { Content = JsonContent.Create(content) });

		httpClientFactory.CreateClient(Arg.Any<string>()).Returns(new HttpClient(handler));
		return handler;
	}

	SpyHttpMessageHandler ReturnContents(params (string Uri, dynamic? Content)[] responses)
	{
		var handler = new SpyHttpMessageHandler(request =>
		{
			var requestUri = request.RequestUri?.ToString();

			foreach (var response in responses)
				if (response.Uri == requestUri)
				{
					var content =
						response.Content is null
							? JsonContent.Create<object?>(null)
							: JsonContent.Create(response.Content);

					return new HttpResponseMessage(HttpStatusCode.OK) { Content = content };
				}

			return new HttpResponseMessage(HttpStatusCode.NotFound);
		});

		httpClientFactory.CreateClient(Arg.Any<string>()).Returns(new HttpClient(handler));
		return handler;
	}

	void Throws(Exception ex)
	{
		var handler = new SpyHttpMessageHandler(_ => throw ex);

		httpClientFactory.CreateClient(Arg.Any<string>()).Returns(new HttpClient(handler));
	}
}
