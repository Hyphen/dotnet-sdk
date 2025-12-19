using System.Net;
using System.Net.Http.Json;
using Hyphen.Sdk.Resources;

namespace Hyphen.Sdk;

[CleanEnvironment(Env.ApiKey, Env.Dev)]
public class NetInfoTests
{
	readonly IHttpClientFactory httpClientFactory;
	readonly Location location;
	readonly SpyLogger<INetInfo> logger;
	readonly NetInfoOptions options = new() { ApiKey = "abc123" };

	public NetInfoTests()
	{
		httpClientFactory = Substitute.For<IHttpClientFactory>();
		logger = new SpyLogger<INetInfo>();
		location = new()
		{
			City = "city",
			Country = "country",
			GeoNameId = 123,
			Latitude = 21.12m,
			Longitude = 42.24m,
			PostalCode = "12345",
			Region = "region",
			TimeZone = "Some/Time_Zone",
		};
	}

	public class Constructor : NetInfoTests
	{
		[Fact]
		public void GuardClauses()
		{
			Assert.Throws<ArgumentNullException>("httpClientFactory", () => new NetInfo(null!, logger, Options.Create(options)));
			Assert.Throws<ArgumentNullException>("logger", () => new NetInfo(httpClientFactory, null!, Options.Create(options)));
			Assert.Throws<ArgumentNullException>("options", () => new NetInfo(httpClientFactory, logger, null!));

			options.ApiKey = null;

			var ex1 = Assert.Throws<ApiKeyException>(() => new NetInfo(httpClientFactory, logger, Options.Create(options)));
			Assert.Equal("API key is required. Please provide it via options or set the HYPHEN_API_KEY environment variable.", ex1.Message);

			options.ApiKey = "public_abc123";

			var ex2 = Assert.Throws<ApiKeyException>(() => new NetInfo(httpClientFactory, logger, Options.Create(options)));
			Assert.Equal("The provided API key is a public API key. Please provide a valid non public API key for authentication.", ex2.Message);
		}

		[Theory]
		[InlineData("false", "https://net.info/")]
		[InlineData("true", "https://dev.net.info/")]
		public void UsesDefaultBaseUri(string hyphenDevValue, string expectedUri)
		{
			Environment.SetEnvironmentVariable(Env.Dev, hyphenDevValue);

			var netInfo = new NetInfo(httpClientFactory, logger, Options.Create(options));

			Assert.Equal(expectedUri, netInfo.BaseUri.ToString());
		}

		[Theory]
		[InlineData("http://localhost:12345/foo")]
		[InlineData("http://localhost:12345/foo/")]
		public void NormalizesTrailingSlashOnBaseUri(string baseUri)
		{
			options.BaseUri = new(baseUri);

			var netInfo = new NetInfo(httpClientFactory, logger, Options.Create(options));

			Assert.Equal("http://localhost:12345/foo/", netInfo.BaseUri.ToString());
		}
	}

	public class CurrentIP : NetInfoTests
	{
		[Fact]
		public async ValueTask Returns200()
		{
			var netInfo = new NetInfo(httpClientFactory, logger, Options.Create(options));
			var ip = new NetInfoResult { IP = "1.2.3.4", Type = IPType.Error, ErrorMessage = "Unknown IP address" };
			var messageHandler = ReturnContent200(ip);

			var response = await netInfo.GetIPInfo(TestContext.Current.CancellationToken);

			var request = Assert.Single(messageHandler.Requests);
			Assert.Equal(HttpMethod.Get, request.Method);
			Assert.Equal("https://net.info/ip", request.Uri);
			Assert.Equal("abc123", Assert.Single(request.Headers.GetValues("x-api-key")));
			Assert.Null(request.Body);
			Assert.Equivalent(ip, response, strict: true);
		}

		[Fact]
		public async ValueTask Returns200_MalformedResponses()
		{
			var netInfo = new NetInfo(httpClientFactory, logger, Options.Create(options));

			async ValueTask assertFailure(Action setup)
			{
				setup();

				var response = await netInfo.GetIPInfo(TestContext.Current.CancellationToken);

				Assert.Equivalent(new
				{
					IP = "unknown",
					Type = IPType.Error,
					ErrorMessage = HyphenSdkResources.Http_ResponseMalformed,
					Location = default(Location)
				}, response, strict: true);
			}

			await assertFailure(() => ReturnContent200(default(object)));
			await assertFailure(() => ReturnContent200(new { }));
			await assertFailure(() => ReturnContent200(Array.Empty<object>()));
		}

		[Fact]
		public async ValueTask ReturnsNon200()
		{
			var netInfo = new NetInfo(httpClientFactory, logger, Options.Create(options));
			ReturnStatusCode(HttpStatusCode.NotFound);

			var response = await netInfo.GetIPInfo(TestContext.Current.CancellationToken);

			Assert.Equivalent(new
			{
				IP = "unknown",
				Type = IPType.Error,
				ErrorMessage = HyphenSdkResources.Http_StatusCodeError(HttpStatusCode.NotFound),
				Location = default(Location)
			}, response, strict: true);
		}

		[Fact]
		public async ValueTask Throws()
		{
			var netInfo = new NetInfo(httpClientFactory, logger, Options.Create(options));
			ThrowResponse(new DivideByZeroException());

			var response = await netInfo.GetIPInfo(TestContext.Current.CancellationToken);

			Assert.Equivalent(new
			{
				IP = "unknown",
				Type = IPType.Error,
				ErrorMessage = "System.DivideByZeroException: Attempted to divide by zero.",
				Location = default(Location)
			}, response, strict: true);
		}
	}

	public class SingleIP : NetInfoTests
	{
		[Fact]
		public async ValueTask Returns200()
		{
			var netInfo = new NetInfo(httpClientFactory, logger, Options.Create(options));
			var ip = new NetInfoResult { IP = "1.2.3.4", Type = IPType.Error, ErrorMessage = "Unknown IP address" };
			var result = new NetInfoPostResponse200 { Data = [ip] };
			var messageHandler = ReturnContent200(result);

			var response = await netInfo.GetIPInfo("1.2.3.4", TestContext.Current.CancellationToken);

			var request = Assert.Single(messageHandler.Requests);
			Assert.Equal(HttpMethod.Post, request.Method);
			Assert.Equal("https://net.info/ip", request.Uri);
			Assert.Equal("abc123", Assert.Single(request.Headers.GetValues("x-api-key")));
			Assert.Equal("""["1.2.3.4"]""", request.Body);
			Assert.Equivalent(ip, response, strict: true);
		}

		[Fact]
		public async ValueTask Returns200_MalformedResponses()
		{
			var netInfo = new NetInfo(httpClientFactory, logger, Options.Create(options));

			async ValueTask assertFailure(Action setup)
			{
				setup();

				var response = await netInfo.GetIPInfo("1.2.3.4", TestContext.Current.CancellationToken);

				Assert.Equivalent(new
				{
					IP = "1.2.3.4",
					Type = IPType.Error,
					ErrorMessage = HyphenSdkResources.Http_ResponseMalformed,
					Location = default(Location)
				}, response, strict: true);
			}

			await assertFailure(() => ReturnContent200(default(object)));
			await assertFailure(() => ReturnContent200(new { }));
			await assertFailure(() => ReturnContent200(Array.Empty<object>()));
			await assertFailure(() => ReturnContent200(new NetInfoPostResponse200 { Data = [null!] }));
		}

		[Fact]
		public async ValueTask ReturnsNon200()
		{
			var netInfo = new NetInfo(httpClientFactory, logger, Options.Create(options));
			ReturnStatusCode(HttpStatusCode.NotFound);

			var response = await netInfo.GetIPInfo("1.2.3.4", TestContext.Current.CancellationToken);

			Assert.Equivalent(new
			{
				IP = "1.2.3.4",
				Type = IPType.Error,
				ErrorMessage = HyphenSdkResources.Http_StatusCodeError(HttpStatusCode.NotFound),
				Location = default(Location)
			}, response, strict: true);
		}

		[Fact]
		public async ValueTask Throws()
		{
			var netInfo = new NetInfo(httpClientFactory, logger, Options.Create(options));
			ThrowResponse(new DivideByZeroException());

			var response = await netInfo.GetIPInfo("1.2.3.4", TestContext.Current.CancellationToken);

			Assert.Equivalent(new
			{
				IP = "1.2.3.4",
				Type = IPType.Error,
				ErrorMessage = "System.DivideByZeroException: Attempted to divide by zero.",
				Location = default(Location)
			}, response, strict: true);
		}
	}

	public class MultipleIPs : NetInfoTests
	{
		[Fact]
		public async ValueTask GuardClause()
		{
			var netInfo = new NetInfo(httpClientFactory, logger, Options.Create(options));

			await Assert.ThrowsAsync<ArgumentNullException>("ips", () => netInfo.GetIPInfos(null!, TestContext.Current.CancellationToken));
		}

		[Fact]
		public async ValueTask Returns200()
		{
			var netInfo = new NetInfo(httpClientFactory, logger, Options.Create(options));
			var ip1 = new NetInfoResult { IP = "1.2.3.4", Type = IPType.Private };
			var ip2 = new NetInfoResult { IP = "5.6.7.8", Type = IPType.Public, Location = location };
			var result = new NetInfoPostResponse200 { Data = [ip1, ip2] };
			var messageHandler = ReturnContent200(result);

			var response = await netInfo.GetIPInfos(["1.2.3.4", "5.6.7.8"], TestContext.Current.CancellationToken);

			var request = Assert.Single(messageHandler.Requests);
			Assert.Equal(HttpMethod.Post, request.Method);
			Assert.Equal("https://net.info/ip", request.Uri);
			Assert.Equal("abc123", Assert.Single(request.Headers.GetValues("x-api-key")));
			Assert.Equal("""["1.2.3.4","5.6.7.8"]""", request.Body);
			Assert.Equivalent(ip1, response[0], strict: true);
			Assert.Equivalent(ip2, response[1], strict: true);
		}

		[Fact]
		public async ValueTask Returns200_MalformedResponses()
		{
			var netInfo = new NetInfo(httpClientFactory, logger, Options.Create(options));

			async ValueTask assertFailure(Action setup)
			{
				setup();

				var response = await netInfo.GetIPInfos(["1.2.3.4", "5.6.7.8"], TestContext.Current.CancellationToken);

				Assert.Collection(response,
					item =>
						Assert.Equivalent(new
						{
							IP = "1.2.3.4",
							Type = IPType.Error,
							ErrorMessage = HyphenSdkResources.Http_ResponseMalformed,
							Location = default(Location)
						}, item, strict: true),
					item =>
						Assert.Equivalent(new
						{
							IP = "5.6.7.8",
							Type = IPType.Error,
							ErrorMessage = HyphenSdkResources.Http_ResponseMalformed,
							Location = default(Location)
						}, item, strict: true)
				);
			}

			await assertFailure(() => ReturnContent200(default(object)));
			await assertFailure(() => ReturnContent200(new { }));
			await assertFailure(() => ReturnContent200(Array.Empty<object>()));
			await assertFailure(() => ReturnContent200(new NetInfoPostResponse200 { Data = [null!, null!] }));
		}

		[Fact]
		public async ValueTask Returns200_MalformedResponse_TooManyElementsInResult()
		{
			var netInfo = new NetInfo(httpClientFactory, logger, Options.Create(options));
			ReturnContent200(new NetInfoPostResponse200 { Data = [null!, null!, null!] });

			var response = await netInfo.GetIPInfos(["1.2.3.4", "5.6.7.8"], TestContext.Current.CancellationToken);

			Assert.Collection(response,
				item =>
					Assert.Equivalent(new
					{
						IP = "1.2.3.4",
						Type = IPType.Error,
						ErrorMessage = HyphenSdkResources.Http_ResponseMalformed,
						Location = default(Location)
					}, item, strict: true),
				item =>
					Assert.Equivalent(new
					{
						IP = "5.6.7.8",
						Type = IPType.Error,
						ErrorMessage = HyphenSdkResources.Http_ResponseMalformed,
						Location = default(Location)
					}, item, strict: true),
				item =>
					Assert.Equivalent(new
					{
						IP = "unknown",
						Type = IPType.Error,
						ErrorMessage = HyphenSdkResources.Http_ResponseMalformed,
						Location = default(Location)
					}, item, strict: true)
			);
		}

		[Fact]
		public async ValueTask ReturnsNon200()
		{
			var netInfo = new NetInfo(httpClientFactory, logger, Options.Create(options));
			ReturnStatusCode(HttpStatusCode.NotFound);

			var response = await netInfo.GetIPInfos(["1.2.3.4", "5.6.7.8"], TestContext.Current.CancellationToken);

			Assert.Collection(response,
				item =>
					Assert.Equivalent(new
					{
						IP = "1.2.3.4",
						Type = IPType.Error,
						ErrorMessage = HyphenSdkResources.Http_StatusCodeError(HttpStatusCode.NotFound),
						Location = default(Location)
					}, item, strict: true),
				item =>
					Assert.Equivalent(new
					{
						IP = "5.6.7.8",
						Type = IPType.Error,
						ErrorMessage = HyphenSdkResources.Http_StatusCodeError(HttpStatusCode.NotFound),
						Location = default(Location)
					}, item, strict: true)
			);
		}

		[Fact]
		public async ValueTask Throws()
		{
			var netInfo = new NetInfo(httpClientFactory, logger, Options.Create(options));
			ThrowResponse(new DivideByZeroException());

			var response = await netInfo.GetIPInfos(["1.2.3.4", "5.6.7.8"], TestContext.Current.CancellationToken);

			Assert.Collection(response,
				item =>
					Assert.Equivalent(new
					{
						IP = "1.2.3.4",
						Type = IPType.Error,
						ErrorMessage = "System.DivideByZeroException: Attempted to divide by zero.",
						Location = default(Location)
					}, item, strict: true),
				item =>
					Assert.Equivalent(new
					{
						IP = "5.6.7.8",
						Type = IPType.Error,
						ErrorMessage = "System.DivideByZeroException: Attempted to divide by zero.",
						Location = default(Location)
					}, item, strict: true)
			);
		}
	}

	SpyHttpMessageHandler ReturnContent200<T>(T content)
	{
		var handler = new SpyHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK) { Content = JsonContent.Create(content) });

		httpClientFactory.CreateClient(Arg.Any<string>()).Returns(new HttpClient(handler));
		return handler;
	}

	void ReturnStatusCode(HttpStatusCode statusCode)
	{
		var handler = new SpyHttpMessageHandler(_ => new HttpResponseMessage(statusCode));

		httpClientFactory.CreateClient(Arg.Any<string>()).Returns(new HttpClient(handler));
	}

	void ThrowResponse(Exception ex)
	{
		var handler = new SpyHttpMessageHandler(_ => throw ex);

		httpClientFactory.CreateClient(Arg.Any<string>()).Returns(new HttpClient(handler));
	}
}
