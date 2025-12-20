#pragma warning disable xUnit1047  // Discovery enumeration has been disabled when appropriate

using System.Drawing;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using Hyphen.Sdk.Resources;

namespace Hyphen.Sdk;

[CleanEnvironment(Env.ApiKey, Env.Dev, Env.OrganizationId)]
public class LinkTests
{
	const string OrganizationId = "org_123456789012345678901234";
	const string QrCodeId = "lqr_123456789012345678901234";
	const string ShortCodeId = "code_123456789012345678901234";
	readonly DateTimeOffset StatsStartDate = DateTimeOffset.Now.AddDays(-5);
	readonly DateTimeOffset StatsEndDate = DateTimeOffset.Now;
	readonly string[] Tags = ["tag1", "tag2", "tag3"];

	readonly IHttpClientFactory httpClientFactory;
	readonly SpyLogger<ILink> logger;
	readonly LinkOptions options = new() { ApiKey = "abc123", OrganizationId = OrganizationId };

	public LinkTests()
	{
		httpClientFactory = Substitute.For<IHttpClientFactory>();
		logger = new SpyLogger<ILink>();
	}

	public class Constructor : LinkTests
	{
		[Fact]
		public void GuardClauses()
		{
			Assert.Throws<ArgumentNullException>("httpClientFactory", () => new Link(null!, logger, Options.Create(options)));
			Assert.Throws<ArgumentNullException>("logger", () => new Link(httpClientFactory, null!, Options.Create(options)));
			Assert.Throws<ArgumentNullException>("options", () => new Link(httpClientFactory, logger, null!));

			options.ApiKey = null;

			var ex1 = Assert.Throws<ApiKeyException>(() => new Link(httpClientFactory, logger, Options.Create(options)));
			Assert.Equal("API key is required. Please provide it via options or set the HYPHEN_API_KEY environment variable.", ex1.Message);

			options.ApiKey = "public_abc123";

			var ex2 = Assert.Throws<ApiKeyException>(() => new Link(httpClientFactory, logger, Options.Create(options)));
			Assert.Equal("The provided API key is a public API key. Please provide a valid non public API key for authentication.", ex2.Message);

			options.ApiKey = "abc123";
			options.OrganizationId = null;

			var ex3 = Assert.Throws<ArgumentException>(() => new Link(httpClientFactory, logger, Options.Create(options)));
			Assert.Equal("Organization ID is required. Please provide it via options or set the HYPHEN_ORGANIZATION_ID environment variable.", ex3.Message);
		}

		[Theory]
		[InlineData("false", $"https://api.hyphen.ai/api/organizations/{OrganizationId}/link/codes/")]
		[InlineData("true", $"https://dev-api.hyphen.ai/api/organizations/{OrganizationId}/link/codes/")]
		public void UsesDefaultBaseUri(string hyphenDevValue, string expectedUri)
		{
			Environment.SetEnvironmentVariable(Env.Dev, hyphenDevValue);

			var link = new Link(httpClientFactory, logger, Options.Create(options));

			Assert.Equal(expectedUri, link.BaseUri.ToString());
		}

		[Theory]
		[InlineData("http://localhost:12345/foo")]
		[InlineData("http://localhost:12345/foo/")]
		public void NormalizesTrailingSlashOnBaseUri(string baseUri)
		{
			options.BaseUriTemplate = baseUri;

			var link = new Link(httpClientFactory, logger, Options.Create(options));

			Assert.Equal("http://localhost:12345/foo/", link.BaseUri.ToString());
		}
	}

	public class CreateQrCode : LinkTests
	{
		public static IEnumerable<TheoryDataRow<CreateQrCodeParams?, string?>> ParamsData =
		[
			new(null, null),
			new(
				new()
				{
					BackgroundColor = Color.AliceBlue,
					Color = Color.DarkKhaki,
					Logo = Encoding.UTF8.GetBytes("Hello world"),
					Size = QrCodeSize.Small,
					Title = "QR Code Title",
				},
				"""
				--hyphen-dotnet-sdk
				Content-Type: text/plain; charset=utf-8
				Content-Disposition: form-data; name=backgroundColor

				#f0f8ff
				--hyphen-dotnet-sdk
				Content-Type: text/plain; charset=utf-8
				Content-Disposition: form-data; name=color

				#bdb76b
				--hyphen-dotnet-sdk
				Content-Type: application/octet-stream
				Content-Disposition: form-data; name=logo

				Hello world
				--hyphen-dotnet-sdk
				Content-Type: text/plain; charset=utf-8
				Content-Disposition: form-data; name=size

				small
				--hyphen-dotnet-sdk
				Content-Type: text/plain; charset=utf-8
				Content-Disposition: form-data; name=title

				QR Code Title
				--hyphen-dotnet-sdk--

				"""
			),
		];

		[Theory]
		[MemberData(nameof(ParamsData), DisableDiscoveryEnumeration = true)]
		public async ValueTask Returns201(CreateQrCodeParams? parms, string? expectedPostBody)
		{
			var link = new Link(httpClientFactory, logger, Options.Create(options));
			var qrCode = new
			{
				id = QrCodeId,
				qrCode = "base64_encoded_image",
				qrLink = "http://localhost:12345/",
				title = "QR Code Title",
			};
			var messageHandler = ReturnContent(qrCode, HttpStatusCode.Created);

			var response = await link.CreateQrCode(ShortCodeId, parms, TestContext.Current.CancellationToken);

			var request = Assert.Single(messageHandler.Requests);
			Assert.Equal(HttpMethod.Post, request.Method);
			Assert.Equal($"https://api.hyphen.ai/api/organizations/{OrganizationId}/link/codes/{ShortCodeId}/qrs", request.Uri);
			Assert.Equal("abc123", Assert.Single(request.Headers.GetValues("x-api-key")));
			Assert.Equal(expectedPostBody, request.Body, ignoreLineEndingDifferences: true);
			Assert.Equivalent(new QrCodeResult
			{
				Id = QrCodeId,
				Link = new Uri("http://localhost:12345/"),
				QrCode = "base64_encoded_image",
				Title = "QR Code Title",
			}, response, strict: true);
		}

		[Fact]
		public async ValueTask Returns201_MalformedResponses()
		{
			var link = new Link(httpClientFactory, logger, Options.Create(options));

			async ValueTask assertFailure(Action setup)
			{
				setup();

				var ex = await Record.ExceptionAsync(() => link.CreateQrCode(ShortCodeId, TestContext.Current.CancellationToken));

				// Malformed 200 is presented as a 503, on the assumption the service is broken
				var hscEx = Assert.IsType<HttpStatusCodeException>(ex);
				Assert.Equal(HttpStatusCode.ServiceUnavailable, hscEx.StatusCode);
				Assert.Equal(HyphenSdkResources.Http_StatusCodeError(HttpStatusCode.ServiceUnavailable, HyphenSdkResources.Http_ResponseMalformed), hscEx.Message);
			}

			await assertFailure(() => ReturnContent(default(object), HttpStatusCode.Created));
			await assertFailure(() => ReturnContent(new { }, HttpStatusCode.Created));
			await assertFailure(() => ReturnContent(Array.Empty<object>(), HttpStatusCode.Created));
		}

		[Theory]
		[InlineData(HttpStatusCode.Unauthorized)]
		[InlineData(HttpStatusCode.Forbidden)]
		public async ValueTask Returns401_403(HttpStatusCode statusCode)
		{
			var link = new Link(httpClientFactory, logger, Options.Create(options));
			ReturnStatusCode(statusCode);

			var ex = await Record.ExceptionAsync(() => link.CreateQrCode(ShortCodeId, TestContext.Current.CancellationToken));

			Assert.IsType<ApiKeyException>(ex);
			Assert.Equal(HyphenSdkResources.Http_InvalidApiKey, ex.Message);
		}

		[Fact]
		public async ValueTask Returns404()
		{
			var link = new Link(httpClientFactory, logger, Options.Create(options));
			ReturnStatusCode(HttpStatusCode.NotFound);

			var ex = await Record.ExceptionAsync(() => link.CreateQrCode(ShortCodeId, TestContext.Current.CancellationToken));

			Assert.IsType<NotFoundException>(ex);
			Assert.Equal(HyphenSdkResources.Link_CreateQrCode404, ex.Message);
		}

		[Theory]
		[InlineData(HttpStatusCode.InternalServerError)]
		[InlineData(HttpStatusCode.NotImplemented)]
		public async ValueTask ReturnsNon200(HttpStatusCode statusCode)
		{
			var link = new Link(httpClientFactory, logger, Options.Create(options));
			ReturnStatusCode(statusCode);

			var ex = await Record.ExceptionAsync(() => link.CreateQrCode(ShortCodeId, TestContext.Current.CancellationToken));

			var hscEx = Assert.IsType<HttpStatusCodeException>(ex);
			Assert.Equal(statusCode, hscEx.StatusCode);
			Assert.Equal(HyphenSdkResources.Http_StatusCodeError(statusCode), hscEx.Message);
		}
	}

	public class CreateShortCode : LinkTests
	{
		public static IEnumerable<TheoryDataRow<CreateShortCodeParams?, string>> ParamsData =
		[
			new(
				null,
				"""{"long_url":"http://localhost:12345","domain":"foo.com"}"""
			),
			new(
				new() { Code = "code", Title = "title", Tags = ["tag1", "tag2"] },
				"""{"long_url":"http://localhost:12345","domain":"foo.com","code":"code","title":"title","tags":["tag1","tag2"]}"""
			),
		];

		[Theory]
		[MemberData(nameof(ParamsData))]
		public async ValueTask Returns201(CreateShortCodeParams? parms, string expectedBody)
		{
			var link = new Link(httpClientFactory, logger, Options.Create(options));
			var shortCode = new
			{
				code = "code123",
				domain = "foo.com",
				id = ShortCodeId,
				long_url = "http://localhost:12345/",
				organization = new { id = OrganizationId, name = "Organization Name" },
				tags = Tags,
				title = "Code Title",
			};
			var messageHandler = ReturnContent(shortCode, HttpStatusCode.Created);

			var response = await link.CreateShortCode(new("http://localhost:12345"), "foo.com", parms, TestContext.Current.CancellationToken);

			var request = Assert.Single(messageHandler.Requests);
			Assert.Equal(HttpMethod.Post, request.Method);
			Assert.Equal($"https://api.hyphen.ai/api/organizations/{OrganizationId}/link/codes/", request.Uri);
			Assert.Equal("abc123", Assert.Single(request.Headers.GetValues("x-api-key")));
			Assert.Equal(expectedBody, request.Body, ignoreLineEndingDifferences: true);
			Assert.Equivalent(new ShortCodeResult
			{
				Code = "code123",
				Domain = "foo.com",
				Id = ShortCodeId,
				LongUrl = new Uri("http://localhost:12345/"),
				Organization = new OrganizationResult { Id = OrganizationId, Name = "Organization Name" },
				Tags = Tags,
				Title = "Code Title",
			}, response, strict: true);
		}

		[Fact]
		public async ValueTask Returns201_MalformedResponses()
		{
			var link = new Link(httpClientFactory, logger, Options.Create(options));

			async ValueTask assertFailure(Action setup)
			{
				setup();

				var ex = await Record.ExceptionAsync(() => link.CreateShortCode(new("http://localhost:12345"), "foo.com", TestContext.Current.CancellationToken));

				// Malformed 200 is presented as a 503, on the assumption the service is broken
				var hscEx = Assert.IsType<HttpStatusCodeException>(ex);
				Assert.Equal(HttpStatusCode.ServiceUnavailable, hscEx.StatusCode);
				Assert.Equal(HyphenSdkResources.Http_StatusCodeError(HttpStatusCode.ServiceUnavailable, HyphenSdkResources.Http_ResponseMalformed), hscEx.Message);
			}

			await assertFailure(() => ReturnContent(default(object), HttpStatusCode.Created));
			await assertFailure(() => ReturnContent(new { }, HttpStatusCode.Created));
			await assertFailure(() => ReturnContent(Array.Empty<object>(), HttpStatusCode.Created));
		}

		[Theory]
		[InlineData(HttpStatusCode.Unauthorized)]
		[InlineData(HttpStatusCode.Forbidden)]
		public async ValueTask Returns401_403(HttpStatusCode statusCode)
		{
			var link = new Link(httpClientFactory, logger, Options.Create(options));
			ReturnStatusCode(statusCode);

			var ex = await Record.ExceptionAsync(() => link.CreateShortCode(new("http://localhost:12345"), "foo.com", TestContext.Current.CancellationToken));

			Assert.IsType<ApiKeyException>(ex);
			Assert.Equal(HyphenSdkResources.Http_InvalidApiKey, ex.Message);
		}

		[Fact]
		public async ValueTask Returns404()
		{
			var link = new Link(httpClientFactory, logger, Options.Create(options));
			ReturnStatusCode(HttpStatusCode.NotFound);

			var ex = await Record.ExceptionAsync(() => link.CreateShortCode(new("http://localhost:12345"), "foo.com", TestContext.Current.CancellationToken));

			Assert.IsType<NotFoundException>(ex);
			Assert.Equal(HyphenSdkResources.Link_CreateShortCode404, ex.Message);
		}

		[Theory]
		[InlineData(HttpStatusCode.InternalServerError)]
		[InlineData(HttpStatusCode.NotImplemented)]
		public async ValueTask ReturnsNon200(HttpStatusCode statusCode)
		{
			var link = new Link(httpClientFactory, logger, Options.Create(options));
			ReturnStatusCode(statusCode);

			var ex = await Record.ExceptionAsync(() => link.CreateShortCode(new("http://localhost:12345"), "foo.com", TestContext.Current.CancellationToken));

			var hscEx = Assert.IsType<HttpStatusCodeException>(ex);
			Assert.Equal(statusCode, hscEx.StatusCode);
			Assert.Equal(HyphenSdkResources.Http_StatusCodeError(statusCode), hscEx.Message);
		}
	}

	public class DeleteQrCode : LinkTests
	{
		[Theory]
		[InlineData(HttpStatusCode.NoContent)]
		[InlineData(HttpStatusCode.NotFound)]
		public async ValueTask Returns204_404(HttpStatusCode statusCode)
		{
			var link = new Link(httpClientFactory, logger, Options.Create(options));
			var messageHandler = ReturnStatusCode(statusCode);

			await link.DeleteQrCode(ShortCodeId, QrCodeId, TestContext.Current.CancellationToken);

			var request = Assert.Single(messageHandler.Requests);
			Assert.Equal(HttpMethod.Delete, request.Method);
			Assert.Equal($"https://api.hyphen.ai/api/organizations/{OrganizationId}/link/codes/{ShortCodeId}/qrs/{QrCodeId}", request.Uri);
			Assert.Equal("abc123", Assert.Single(request.Headers.GetValues("x-api-key")));
			Assert.Null(request.Body);
		}

		[Theory]
		[InlineData(HttpStatusCode.Unauthorized)]
		[InlineData(HttpStatusCode.Forbidden)]
		public async ValueTask Returns401_403(HttpStatusCode statusCode)
		{
			var link = new Link(httpClientFactory, logger, Options.Create(options));
			ReturnStatusCode(statusCode);

			var ex = await Record.ExceptionAsync(() => link.DeleteQrCode(ShortCodeId, QrCodeId, TestContext.Current.CancellationToken));

			Assert.IsType<ApiKeyException>(ex);
			Assert.Equal(HyphenSdkResources.Http_InvalidApiKey, ex.Message);
		}

		[Theory]
		[InlineData(HttpStatusCode.InternalServerError)]
		[InlineData(HttpStatusCode.NotImplemented)]
		public async ValueTask ReturnsNon200(HttpStatusCode statusCode)
		{
			var link = new Link(httpClientFactory, logger, Options.Create(options));
			ReturnStatusCode(statusCode);

			var ex = await Record.ExceptionAsync(() => link.DeleteQrCode(ShortCodeId, QrCodeId, TestContext.Current.CancellationToken));

			var hscEx = Assert.IsType<HttpStatusCodeException>(ex);
			Assert.Equal(statusCode, hscEx.StatusCode);
			Assert.Equal(HyphenSdkResources.Http_StatusCodeError(statusCode), hscEx.Message);
		}
	}

	public class DeleteShortCode : LinkTests
	{
		[Theory]
		[InlineData(HttpStatusCode.NoContent)]
		[InlineData(HttpStatusCode.NotFound)]
		public async ValueTask Returns204_404(HttpStatusCode statusCode)
		{
			var link = new Link(httpClientFactory, logger, Options.Create(options));
			var messageHandler = ReturnStatusCode(statusCode);

			await link.DeleteShortCode(ShortCodeId, TestContext.Current.CancellationToken);

			var request = Assert.Single(messageHandler.Requests);
			Assert.Equal(HttpMethod.Delete, request.Method);
			Assert.Equal($"https://api.hyphen.ai/api/organizations/{OrganizationId}/link/codes/{ShortCodeId}", request.Uri);
			Assert.Equal("abc123", Assert.Single(request.Headers.GetValues("x-api-key")));
			Assert.Null(request.Body);
		}

		[Theory]
		[InlineData(HttpStatusCode.Unauthorized)]
		[InlineData(HttpStatusCode.Forbidden)]
		public async ValueTask Returns401_403(HttpStatusCode statusCode)
		{
			var link = new Link(httpClientFactory, logger, Options.Create(options));
			ReturnStatusCode(statusCode);

			var ex = await Record.ExceptionAsync(() => link.DeleteShortCode(ShortCodeId, TestContext.Current.CancellationToken));

			Assert.IsType<ApiKeyException>(ex);
			Assert.Equal(HyphenSdkResources.Http_InvalidApiKey, ex.Message);
		}

		[Theory]
		[InlineData(HttpStatusCode.InternalServerError)]
		[InlineData(HttpStatusCode.NotImplemented)]
		public async ValueTask ReturnsNon200(HttpStatusCode statusCode)
		{
			var link = new Link(httpClientFactory, logger, Options.Create(options));
			ReturnStatusCode(statusCode);

			var ex = await Record.ExceptionAsync(() => link.DeleteShortCode(ShortCodeId, TestContext.Current.CancellationToken));

			var hscEx = Assert.IsType<HttpStatusCodeException>(ex);
			Assert.Equal(statusCode, hscEx.StatusCode);
			Assert.Equal(HyphenSdkResources.Http_StatusCodeError(statusCode), hscEx.Message);
		}
	}

	public class GetQrCode : LinkTests
	{
		[Fact]
		public async ValueTask Returns200()
		{
			var link = new Link(httpClientFactory, logger, Options.Create(options));
			var qrCode = new
			{
				id = QrCodeId,
				qrCode = "base64_encoded_image",
				qrLink = "http://localhost:12345/",
				title = "QR Code Title",
			};
			var messageHandler = ReturnContent(qrCode);

			var response = await link.GetQrCode(ShortCodeId, QrCodeId, TestContext.Current.CancellationToken);

			var request = Assert.Single(messageHandler.Requests);
			Assert.Equal(HttpMethod.Get, request.Method);
			Assert.Equal($"https://api.hyphen.ai/api/organizations/{OrganizationId}/link/codes/{ShortCodeId}/qrs/{QrCodeId}", request.Uri);
			Assert.Equal("abc123", Assert.Single(request.Headers.GetValues("x-api-key")));
			Assert.Null(request.Body);
			Assert.Equivalent(new QrCodeResult
			{
				Id = QrCodeId,
				Link = new Uri("http://localhost:12345/"),
				QrCode = "base64_encoded_image",
				Title = "QR Code Title",
			}, response, strict: true);
		}

		[Fact]
		public async ValueTask Returns200_MalformedResponses()
		{
			var link = new Link(httpClientFactory, logger, Options.Create(options));

			async ValueTask assertFailure(Action setup)
			{
				setup();

				var ex = await Record.ExceptionAsync(() => link.GetQrCode(ShortCodeId, QrCodeId, TestContext.Current.CancellationToken));

				// Malformed 200 is presented as a 503, on the assumption the service is broken
				var hscEx = Assert.IsType<HttpStatusCodeException>(ex);
				Assert.Equal(HttpStatusCode.ServiceUnavailable, hscEx.StatusCode);
				Assert.Equal(HyphenSdkResources.Http_StatusCodeError(HttpStatusCode.ServiceUnavailable, HyphenSdkResources.Http_ResponseMalformed), hscEx.Message);
			}

			await assertFailure(() => ReturnContent(default(object)));
			await assertFailure(() => ReturnContent(new { }));
			await assertFailure(() => ReturnContent(Array.Empty<object>()));
		}

		[Theory]
		[InlineData(HttpStatusCode.Unauthorized)]
		[InlineData(HttpStatusCode.Forbidden)]
		public async ValueTask Returns401_403(HttpStatusCode statusCode)
		{
			var link = new Link(httpClientFactory, logger, Options.Create(options));
			ReturnStatusCode(statusCode);

			var ex = await Record.ExceptionAsync(() => link.GetQrCode(ShortCodeId, QrCodeId, TestContext.Current.CancellationToken));

			Assert.IsType<ApiKeyException>(ex);
			Assert.Equal(HyphenSdkResources.Http_InvalidApiKey, ex.Message);
		}

		[Fact]
		public async ValueTask Returns404()
		{
			var link = new Link(httpClientFactory, logger, Options.Create(options));
			ReturnStatusCode(HttpStatusCode.NotFound);

			var response = await link.GetQrCode(ShortCodeId, QrCodeId, TestContext.Current.CancellationToken);

			Assert.Null(response);
		}

		[Theory]
		[InlineData(HttpStatusCode.InternalServerError)]
		[InlineData(HttpStatusCode.NotImplemented)]
		public async ValueTask ReturnsNon200(HttpStatusCode statusCode)
		{
			var link = new Link(httpClientFactory, logger, Options.Create(options));
			ReturnStatusCode(statusCode);

			var ex = await Record.ExceptionAsync(() => link.GetQrCode(ShortCodeId, QrCodeId, TestContext.Current.CancellationToken));

			var hscEx = Assert.IsType<HttpStatusCodeException>(ex);
			Assert.Equal(statusCode, hscEx.StatusCode);
			Assert.Equal(HyphenSdkResources.Http_StatusCodeError(statusCode), hscEx.Message);
		}
	}

	public class GetQrCodes : LinkTests
	{
		public static IEnumerable<TheoryDataRow<GetQrCodesParams?, string>> ParamsData =>
		[
			new(
				null,
				$"https://api.hyphen.ai/api/organizations/{OrganizationId}/link/codes/{ShortCodeId}/qrs"
			),
			new(
				new() { PageNumber = 24 },
				$"https://api.hyphen.ai/api/organizations/{OrganizationId}/link/codes/{ShortCodeId}/qrs?pageNum=24"
			),
			new(
				new() { PageSize = 42 },
				$"https://api.hyphen.ai/api/organizations/{OrganizationId}/link/codes/{ShortCodeId}/qrs?pageSize=42"
			),
			new(
				new() { PageNumber = 24, PageSize = 42 },
				$"https://api.hyphen.ai/api/organizations/{OrganizationId}/link/codes/{ShortCodeId}/qrs?pageNum=24&pageSize=42"
			),
		];

		[Theory]
		[MemberData(nameof(ParamsData), DisableDiscoveryEnumeration = true)]
		public async ValueTask Returns200(GetQrCodesParams? parms, string expectedUri)
		{
			var link = new Link(httpClientFactory, logger, Options.Create(options));
			var qrCodes = new
			{
				data = new[]
				{
					new
					{
						id = "lqr_000000000000000000000001",
						qrCode = "base64_encoded_image_1",
						qrLink = "http://localhost:12345/",
						title = "QR Code 1",
					},
					new
					{
						id = "lqr_000000000000000000000002",
						qrCode = "base64_encoded_image_2",
						qrLink = "http://localhost:54321/",
						title = "QR Code 2",
					},
				},
				pageNum = 1,
				pageSize = 50,
				total = 2,
			};
			var messageHandler = ReturnContent(qrCodes);

			var response = await link.GetQrCodes(ShortCodeId, parms, TestContext.Current.CancellationToken);

			var request = Assert.Single(messageHandler.Requests);
			Assert.Equal(HttpMethod.Get, request.Method);
			Assert.Equal(expectedUri, request.Uri);
			Assert.Equal("abc123", Assert.Single(request.Headers.GetValues("x-api-key")));
			Assert.Null(request.Body);
			Assert.Equivalent(new PagedResult<QrCodeResult>
			{
				Data =
				[
					new QrCodeResult
					{
						Id = "lqr_000000000000000000000001",
						Link = new Uri("http://localhost:12345/"),
						QrCode = "base64_encoded_image_1",
						Title = "QR Code 1",
					},
					new QrCodeResult
					{
						Id = "lqr_000000000000000000000002",
						Link = new Uri("http://localhost:54321/"),
						QrCode = "base64_encoded_image_2",
						Title = "QR Code 2",
					},
				],
				PageNumber = 1,
				PageSize = 50,
				Total = 2,
			}, response, strict: true);
		}

		[Fact]
		public async ValueTask Returns200_MalformedResponses()
		{
			var link = new Link(httpClientFactory, logger, Options.Create(options));

			async ValueTask assertFailure(Action setup)
			{
				setup();

				var ex = await Record.ExceptionAsync(() => link.GetQrCodes(ShortCodeId, TestContext.Current.CancellationToken));

				// Malformed 200 is presented as a 503, on the assumption the service is broken
				var hscEx = Assert.IsType<HttpStatusCodeException>(ex);
				Assert.Equal(HttpStatusCode.ServiceUnavailable, hscEx.StatusCode);
				Assert.Equal(HyphenSdkResources.Http_StatusCodeError(HttpStatusCode.ServiceUnavailable, HyphenSdkResources.Http_ResponseMalformed), hscEx.Message);
			}

			await assertFailure(() => ReturnContent(default(object)));
			await assertFailure(() => ReturnContent(new { }));
			await assertFailure(() => ReturnContent(Array.Empty<object>()));
		}

		[Theory]
		[InlineData(HttpStatusCode.Unauthorized)]
		[InlineData(HttpStatusCode.Forbidden)]
		public async ValueTask Returns401_403(HttpStatusCode statusCode)
		{
			var link = new Link(httpClientFactory, logger, Options.Create(options));
			ReturnStatusCode(statusCode);

			var ex = await Record.ExceptionAsync(() => link.GetQrCodes(ShortCodeId, TestContext.Current.CancellationToken));

			Assert.IsType<ApiKeyException>(ex);
			Assert.Equal(HyphenSdkResources.Http_InvalidApiKey, ex.Message);
		}

		[Fact]
		public async ValueTask Returns404()
		{
			var link = new Link(httpClientFactory, logger, Options.Create(options));
			ReturnStatusCode(HttpStatusCode.NotFound);

			var response = await link.GetQrCodes(ShortCodeId, TestContext.Current.CancellationToken);

			Assert.Null(response);
		}

		[Theory]
		[InlineData(HttpStatusCode.InternalServerError)]
		[InlineData(HttpStatusCode.NotImplemented)]
		public async ValueTask ReturnsNon200(HttpStatusCode statusCode)
		{
			var link = new Link(httpClientFactory, logger, Options.Create(options));
			ReturnStatusCode(statusCode);

			var ex = await Record.ExceptionAsync(() => link.GetQrCodes(ShortCodeId, TestContext.Current.CancellationToken));

			var hscEx = Assert.IsType<HttpStatusCodeException>(ex);
			Assert.Equal(statusCode, hscEx.StatusCode);
			Assert.Equal(HyphenSdkResources.Http_StatusCodeError(statusCode), hscEx.Message);
		}
	}

	public class GetShortCode : LinkTests
	{
		[Fact]
		public async ValueTask Returns200()
		{
			var link = new Link(httpClientFactory, logger, Options.Create(options));
			var shortCode = new
			{
				code = "code123",
				domain = "foo.com",
				id = ShortCodeId,
				long_url = "http://localhost:12345/",
				organization = new { id = OrganizationId, name = "Organization Name" },
				tags = Tags,
				title = "Code Title",
			};
			var messageHandler = ReturnContent(shortCode);

			var response = await link.GetShortCode(ShortCodeId, TestContext.Current.CancellationToken);

			var request = Assert.Single(messageHandler.Requests);
			Assert.Equal(HttpMethod.Get, request.Method);
			Assert.Equal($"https://api.hyphen.ai/api/organizations/{OrganizationId}/link/codes/{ShortCodeId}", request.Uri);
			Assert.Equal("abc123", Assert.Single(request.Headers.GetValues("x-api-key")));
			Assert.Null(request.Body);
			Assert.Equivalent(new ShortCodeResult
			{
				Code = "code123",
				Domain = "foo.com",
				Id = ShortCodeId,
				LongUrl = new Uri("http://localhost:12345/"),
				Organization = new OrganizationResult { Id = OrganizationId, Name = "Organization Name" },
				Tags = Tags,
				Title = "Code Title",
			}, response, strict: true);
		}

		[Fact]
		public async ValueTask Returns200_MalformedResponses()
		{
			var link = new Link(httpClientFactory, logger, Options.Create(options));

			async ValueTask assertFailure(Action setup)
			{
				setup();

				var ex = await Record.ExceptionAsync(() => link.GetShortCode(ShortCodeId, TestContext.Current.CancellationToken));

				// Malformed 200 is presented as a 503, on the assumption the service is broken
				var hscEx = Assert.IsType<HttpStatusCodeException>(ex);
				Assert.Equal(HttpStatusCode.ServiceUnavailable, hscEx.StatusCode);
				Assert.Equal(HyphenSdkResources.Http_StatusCodeError(HttpStatusCode.ServiceUnavailable, HyphenSdkResources.Http_ResponseMalformed), hscEx.Message);
			}

			await assertFailure(() => ReturnContent(default(object)));
			await assertFailure(() => ReturnContent(new { }));
			await assertFailure(() => ReturnContent(Array.Empty<object>()));
		}

		[Theory]
		[InlineData(HttpStatusCode.Unauthorized)]
		[InlineData(HttpStatusCode.Forbidden)]
		public async ValueTask Returns401_403(HttpStatusCode statusCode)
		{
			var link = new Link(httpClientFactory, logger, Options.Create(options));
			ReturnStatusCode(statusCode);

			var ex = await Record.ExceptionAsync(() => link.GetShortCode(ShortCodeId, TestContext.Current.CancellationToken));

			Assert.IsType<ApiKeyException>(ex);
			Assert.Equal(HyphenSdkResources.Http_InvalidApiKey, ex.Message);
		}

		[Fact]
		public async ValueTask Returns404()
		{
			var link = new Link(httpClientFactory, logger, Options.Create(options));
			ReturnStatusCode(HttpStatusCode.NotFound);

			var response = await link.GetShortCode(ShortCodeId, TestContext.Current.CancellationToken);

			Assert.Null(response);
		}

		[Theory]
		[InlineData(HttpStatusCode.InternalServerError)]
		[InlineData(HttpStatusCode.NotImplemented)]
		public async ValueTask ReturnsNon200(HttpStatusCode statusCode)
		{
			var link = new Link(httpClientFactory, logger, Options.Create(options));
			ReturnStatusCode(statusCode);

			var ex = await Record.ExceptionAsync(() => link.GetShortCode(ShortCodeId, TestContext.Current.CancellationToken));

			var hscEx = Assert.IsType<HttpStatusCodeException>(ex);
			Assert.Equal(statusCode, hscEx.StatusCode);
			Assert.Equal(HyphenSdkResources.Http_StatusCodeError(statusCode), hscEx.Message);
		}
	}

	public class GetShortCodes : LinkTests
	{
		public static IEnumerable<TheoryDataRow<GetShortCodesParams?, string>> ParamsData =>
		[
			new(
				null,
				$"https://api.hyphen.ai/api/organizations/{OrganizationId}/link/codes/"
			),
			new(
				new() { PageNumber = 24, PageSize = 42 },
				$"https://api.hyphen.ai/api/organizations/{OrganizationId}/link/codes/?pageNum=24&pageSize=42"
			),
			new(
				new() { PageNumber = 24, PageSize = 42, Search = "Foo", Tags = ["bar", "baz"] },
				$"https://api.hyphen.ai/api/organizations/{OrganizationId}/link/codes/?pageNum=24&pageSize=42&search=Foo&tags=bar%2cbaz"
			),
		];

		[Theory]
		[MemberData(nameof(ParamsData), DisableDiscoveryEnumeration = true)]
		public async ValueTask Returns200(GetShortCodesParams? parms, string expectedUri)
		{
			var link = new Link(httpClientFactory, logger, Options.Create(options));
			var shortCodes = new
			{
				data = new[]
				{
					new {
						code = "code1",
						domain = "foo.com",
						id = "code_000000000000000000000001",
						long_url = "http://localhost:12345/",
						organization = new { id = OrganizationId, name = "Organization Name" },
						tags = Tags,
						title = "Code Title 1",
					},
					new {
						code = "code2",
						domain = "bar.com",
						id = "code_000000000000000000000002",
						long_url = "http://localhost:54321/",
						organization = new { id = OrganizationId, name = "Organization Name" },
						tags = Array.Empty<string>(),
						title = "Code Title 2",
					},
				},
				pageNum = 1,
				pageSize = 50,
				total = 2,
			};
			var messageHandler = ReturnContent(shortCodes);

			var response = await link.GetShortCodes(parms, TestContext.Current.CancellationToken);

			var request = Assert.Single(messageHandler.Requests);
			Assert.Equal(HttpMethod.Get, request.Method);
			Assert.Equal(expectedUri, request.Uri);
			Assert.Equal("abc123", Assert.Single(request.Headers.GetValues("x-api-key")));
			Assert.Null(request.Body);
			Assert.Equivalent(new PagedResult<ShortCodeResult>
			{
				Data =
				[
					new ShortCodeResult
					{
						Code = "code1",
						Domain = "foo.com",
						Id = "code_000000000000000000000001",
						LongUrl = new Uri("http://localhost:12345/"),
						Organization = new OrganizationResult { Id = OrganizationId, Name = "Organization Name" },
						Tags = Tags,
						Title = "Code Title 1",
					},
					new ShortCodeResult
					{
						Code = "code2",
						Domain = "bar.com",
						Id = "code_000000000000000000000002",
						LongUrl = new Uri("http://localhost:54321/"),
						Organization = new OrganizationResult { Id = OrganizationId, Name = "Organization Name" },
						Tags = [],
						Title = "Code Title 2",
					},
				],
				PageNumber = 1,
				PageSize = 50,
				Total = 2,
			}, response, strict: true);
		}

		[Fact]
		public async ValueTask Returns200_MalformedResponses()
		{
			var link = new Link(httpClientFactory, logger, Options.Create(options));

			async ValueTask assertFailure(Action setup)
			{
				setup();

				var ex = await Record.ExceptionAsync(() => link.GetShortCodes(TestContext.Current.CancellationToken));

				// Malformed 200 is presented as a 503, on the assumption the service is broken
				var hscEx = Assert.IsType<HttpStatusCodeException>(ex);
				Assert.Equal(HttpStatusCode.ServiceUnavailable, hscEx.StatusCode);
				Assert.Equal(HyphenSdkResources.Http_StatusCodeError(HttpStatusCode.ServiceUnavailable, HyphenSdkResources.Http_ResponseMalformed), hscEx.Message);
			}

			await assertFailure(() => ReturnContent(default(object)));
			await assertFailure(() => ReturnContent(new { }));
			await assertFailure(() => ReturnContent(Array.Empty<object>()));
		}

		[Theory]
		[InlineData(HttpStatusCode.Unauthorized)]
		[InlineData(HttpStatusCode.Forbidden)]
		public async ValueTask Returns401_403(HttpStatusCode statusCode)
		{
			var link = new Link(httpClientFactory, logger, Options.Create(options));
			ReturnStatusCode(statusCode);

			var ex = await Record.ExceptionAsync(() => link.GetShortCodes(TestContext.Current.CancellationToken));

			Assert.IsType<ApiKeyException>(ex);
			Assert.Equal(HyphenSdkResources.Http_InvalidApiKey, ex.Message);
		}

		[Fact]
		public async ValueTask Returns404()
		{
			var link = new Link(httpClientFactory, logger, Options.Create(options));
			ReturnStatusCode(HttpStatusCode.NotFound);

			var response = await link.GetShortCodes(TestContext.Current.CancellationToken);

			Assert.Null(response);
		}

		[Theory]
		[InlineData(HttpStatusCode.InternalServerError)]
		[InlineData(HttpStatusCode.NotImplemented)]
		public async ValueTask ReturnsNon200(HttpStatusCode statusCode)
		{
			var link = new Link(httpClientFactory, logger, Options.Create(options));
			ReturnStatusCode(statusCode);

			var ex = await Record.ExceptionAsync(() => link.GetShortCodes(TestContext.Current.CancellationToken));

			var hscEx = Assert.IsType<HttpStatusCodeException>(ex);
			Assert.Equal(statusCode, hscEx.StatusCode);
			Assert.Equal(HyphenSdkResources.Http_StatusCodeError(statusCode), hscEx.Message);
		}
	}

	public class GetShortCodeStats : LinkTests
	{
		[Fact]
		public async ValueTask Returns200()
		{
			var link = new Link(httpClientFactory, logger, Options.Create(options));
			var stats = new
			{
				browsers = new[] { new { name = "Chrome", total = 5 }, new { name = "Vivaldi", total = 12 } },
				clicks = new
				{
					byDay = new[]
					{
						new { date = "2025-12-18T00:00:00.000Z", total = 2100, unique = 40 },
						new { date = "2025-12-19T00:00:00.000Z", total = 12, unique = 2 },
					},
					total = 2112,
					unique = 42,
				},
				devices = new[] { new { name = "Desktop", total = 18 }, new { name = "Phone", total = 6 } },
				locations = new[] { new { country = "US", total = 24, unique = 12 }, new { country = "Canada", total = 16, unique = 8 } },
				referrals = new[] { new { url = "http://localhost:12345/", total = 50 }, new { url = "http://localhost:54321/", total = 75 } },
			};
			var messageHandler = ReturnContent(stats);

			var response = await link.GetShortCodeStats(ShortCodeId, StatsStartDate, StatsEndDate, TestContext.Current.CancellationToken);

			var request = Assert.Single(messageHandler.Requests);
			Assert.Equal(HttpMethod.Get, request.Method);
			Assert.Equal($"https://api.hyphen.ai/api/organizations/{OrganizationId}/link/codes/{ShortCodeId}/stats?startDate={StatsStartDate.ToISO8601()}&endDate={StatsEndDate.ToISO8601()}", request.Uri);
			Assert.Equal("abc123", Assert.Single(request.Headers.GetValues("x-api-key")));
			Assert.Null(request.Body);
			Assert.Equivalent(new ShortCodeStatsResult
			{
				Browsers = [new() { Name = "Chrome", Total = 5 }, new() { Name = "Vivaldi", Total = 12 }],
				Clicks = new()
				{
					Total = 2112,
					Unique = 42,
					ByDay =
					[
						new LinkClicksByDay { Date = DateTimeOffset.Parse("2025-12-18T00:00:00.000Z"), Total = 2100, Unique = 40 },
						new LinkClicksByDay { Date = DateTimeOffset.Parse("2025-12-19T00:00:00.0000000+00:00"), Total = 12, Unique = 2 },
					]
				},
				Devices = [new() { Name = "Desktop", Total = 18 }, new() { Name = "Phone", Total = 6 }],
				Locations = [new() { Country = "US", Total = 24, Unique = 12 }, new() { Country = "Canada", Total = 16, Unique = 8 }],
				Referrals = [new() { Total = 50, Url = new("http://localhost:12345/") }, new() { Total = 75, Url = new("http://localhost:54321/") }],
			}, response, strict: true);
		}

		[Fact]
		public async ValueTask Returns200_MalformedResponses()
		{
			var link = new Link(httpClientFactory, logger, Options.Create(options));

			async ValueTask assertFailure(Action setup)
			{
				setup();

				var ex = await Record.ExceptionAsync(() => link.GetShortCodeStats(ShortCodeId, StatsStartDate, StatsEndDate, TestContext.Current.CancellationToken));

				// Malformed 200 is presented as a 503, on the assumption the service is broken
				var hscEx = Assert.IsType<HttpStatusCodeException>(ex);
				Assert.Equal(HttpStatusCode.ServiceUnavailable, hscEx.StatusCode);
				Assert.Equal(HyphenSdkResources.Http_StatusCodeError(HttpStatusCode.ServiceUnavailable, HyphenSdkResources.Http_ResponseMalformed), hscEx.Message);
			}

			await assertFailure(() => ReturnContent(default(object)));
			await assertFailure(() => ReturnContent(new { }));
			await assertFailure(() => ReturnContent(Array.Empty<object>()));
		}

		[Theory]
		[InlineData(HttpStatusCode.Unauthorized)]
		[InlineData(HttpStatusCode.Forbidden)]
		public async ValueTask Returns401_403(HttpStatusCode statusCode)
		{
			var link = new Link(httpClientFactory, logger, Options.Create(options));
			ReturnStatusCode(statusCode);

			var ex = await Record.ExceptionAsync(() => link.GetShortCodeStats(ShortCodeId, StatsStartDate, StatsEndDate, TestContext.Current.CancellationToken));

			Assert.IsType<ApiKeyException>(ex);
			Assert.Equal(HyphenSdkResources.Http_InvalidApiKey, ex.Message);
		}

		[Fact]
		public async ValueTask Returns404()
		{
			var link = new Link(httpClientFactory, logger, Options.Create(options));
			ReturnStatusCode(HttpStatusCode.NotFound);

			var response = await link.GetShortCodeStats(ShortCodeId, StatsStartDate, StatsEndDate, TestContext.Current.CancellationToken);

			Assert.Null(response);
		}

		[Theory]
		[InlineData(HttpStatusCode.InternalServerError)]
		[InlineData(HttpStatusCode.NotImplemented)]
		public async ValueTask ReturnsNon200(HttpStatusCode statusCode)
		{
			var link = new Link(httpClientFactory, logger, Options.Create(options));
			ReturnStatusCode(statusCode);

			var ex = await Record.ExceptionAsync(() => link.GetShortCodeStats(ShortCodeId, StatsStartDate, StatsEndDate, TestContext.Current.CancellationToken));

			var hscEx = Assert.IsType<HttpStatusCodeException>(ex);
			Assert.Equal(statusCode, hscEx.StatusCode);
			Assert.Equal(HyphenSdkResources.Http_StatusCodeError(statusCode), hscEx.Message);
		}
	}

	public class GetTags : LinkTests
	{
		[Fact]
		public async ValueTask Returns200()
		{
			var link = new Link(httpClientFactory, logger, Options.Create(options));
			var messageHandler = ReturnContent(Tags);

			var response = await link.GetTags(TestContext.Current.CancellationToken);

			var request = Assert.Single(messageHandler.Requests);
			Assert.Equal(HttpMethod.Get, request.Method);
			Assert.Equal("https://api.hyphen.ai/api/organizations/org_123456789012345678901234/link/codes/tags", request.Uri);
			Assert.Equal("abc123", Assert.Single(request.Headers.GetValues("x-api-key")));
			Assert.Null(request.Body);
			Assert.Equal(Tags, response);
		}

		[Fact]
		public async ValueTask Returns200_MalformedResponses()
		{
			var link = new Link(httpClientFactory, logger, Options.Create(options));

			async ValueTask assertFailure(Action setup)
			{
				setup();

				var ex = await Record.ExceptionAsync(() => link.GetTags(TestContext.Current.CancellationToken));

				// Malformed 200 is presented as a 503, on the assumption the service is broken
				var hscEx = Assert.IsType<HttpStatusCodeException>(ex);
				Assert.Equal(HttpStatusCode.ServiceUnavailable, hscEx.StatusCode);
				Assert.Equal(HyphenSdkResources.Http_StatusCodeError(HttpStatusCode.ServiceUnavailable, HyphenSdkResources.Http_ResponseMalformed), hscEx.Message);
			}

			await assertFailure(() => ReturnContent(default(object)));
			await assertFailure(() => ReturnContent(new { }));
			await assertFailure(() => ReturnContent(new[] { 42, 2112 }));
		}

		[Theory]
		[InlineData(HttpStatusCode.Unauthorized)]
		[InlineData(HttpStatusCode.Forbidden)]
		public async ValueTask Returns401_403(HttpStatusCode statusCode)
		{
			var link = new Link(httpClientFactory, logger, Options.Create(options));
			ReturnStatusCode(statusCode);

			var ex = await Record.ExceptionAsync(() => link.GetTags(TestContext.Current.CancellationToken));

			Assert.IsType<ApiKeyException>(ex);
			Assert.Equal(HyphenSdkResources.Http_InvalidApiKey, ex.Message);
		}

		[Fact]
		public async ValueTask Returns404()
		{
			var link = new Link(httpClientFactory, logger, Options.Create(options));
			ReturnStatusCode(HttpStatusCode.NotFound);

			var response = await link.GetTags(TestContext.Current.CancellationToken);

			Assert.Null(response);
		}

		[Theory]
		[InlineData(HttpStatusCode.InternalServerError)]
		[InlineData(HttpStatusCode.NotImplemented)]
		public async ValueTask ReturnsNon200(HttpStatusCode statusCode)
		{
			var link = new Link(httpClientFactory, logger, Options.Create(options));
			ReturnStatusCode(statusCode);

			var ex = await Record.ExceptionAsync(() => link.GetTags(TestContext.Current.CancellationToken));

			var hscEx = Assert.IsType<HttpStatusCodeException>(ex);
			Assert.Equal(statusCode, hscEx.StatusCode);
			Assert.Equal(HyphenSdkResources.Http_StatusCodeError(statusCode), hscEx.Message);
		}
	}

	public class UpdateShortCode : LinkTests
	{
		public static IEnumerable<TheoryDataRow<UpdateShortCodeParams, string>> ParamsData =
		[
			new(new(), "{}"),
			new(
				new() { LongUrl = new("http://localhost:12345"), Title = "title", Tags = ["tag1", "tag2"] },
				"""{"long_url":"http://localhost:12345","title":"title","tags":["tag1","tag2"]}"""
			),
		];

		[Theory]
		[MemberData(nameof(ParamsData))]
		public async ValueTask Returns200(UpdateShortCodeParams parms, string expectedBody)
		{
			var link = new Link(httpClientFactory, logger, Options.Create(options));
			var shortCode = new
			{
				code = "code123",
				domain = "foo.com",
				id = ShortCodeId,
				long_url = "http://localhost:12345/",
				organization = new { id = OrganizationId, name = "Organization Name" },
				tags = Tags,
				title = "Code Title",
			};
			var messageHandler = ReturnContent(shortCode);

			var response = await link.UpdateShortCode(ShortCodeId, parms, TestContext.Current.CancellationToken);

			var request = Assert.Single(messageHandler.Requests);
			Assert.Equal(HttpMethod.Patch, request.Method);
			Assert.Equal($"https://api.hyphen.ai/api/organizations/{OrganizationId}/link/codes/{ShortCodeId}", request.Uri);
			Assert.Equal("abc123", Assert.Single(request.Headers.GetValues("x-api-key")));
			Assert.Equal(expectedBody, request.Body, ignoreLineEndingDifferences: true);
			Assert.Equivalent(new ShortCodeResult
			{
				Code = "code123",
				Domain = "foo.com",
				Id = ShortCodeId,
				LongUrl = new Uri("http://localhost:12345/"),
				Organization = new OrganizationResult { Id = OrganizationId, Name = "Organization Name" },
				Tags = Tags,
				Title = "Code Title",
			}, response, strict: true);
		}

		[Fact]
		public async ValueTask Returns200_MalformedResponses()
		{
			var link = new Link(httpClientFactory, logger, Options.Create(options));

			async ValueTask assertFailure(Action setup)
			{
				setup();

				var ex = await Record.ExceptionAsync(() => link.UpdateShortCode(ShortCodeId, new(), TestContext.Current.CancellationToken));

				// Malformed 200 is presented as a 503, on the assumption the service is broken
				var hscEx = Assert.IsType<HttpStatusCodeException>(ex);
				Assert.Equal(HttpStatusCode.ServiceUnavailable, hscEx.StatusCode);
				Assert.Equal(HyphenSdkResources.Http_StatusCodeError(HttpStatusCode.ServiceUnavailable, HyphenSdkResources.Http_ResponseMalformed), hscEx.Message);
			}

			await assertFailure(() => ReturnContent(default(object)));
			await assertFailure(() => ReturnContent(new { }));
			await assertFailure(() => ReturnContent(Array.Empty<object>()));
		}

		[Theory]
		[InlineData(HttpStatusCode.Unauthorized)]
		[InlineData(HttpStatusCode.Forbidden)]
		public async ValueTask Returns401_403(HttpStatusCode statusCode)
		{
			var link = new Link(httpClientFactory, logger, Options.Create(options));
			ReturnStatusCode(statusCode);

			var ex = await Record.ExceptionAsync(() => link.UpdateShortCode(ShortCodeId, new(), TestContext.Current.CancellationToken));

			Assert.IsType<ApiKeyException>(ex);
			Assert.Equal(HyphenSdkResources.Http_InvalidApiKey, ex.Message);
		}

		[Fact]
		public async ValueTask Returns404()
		{
			var link = new Link(httpClientFactory, logger, Options.Create(options));
			ReturnStatusCode(HttpStatusCode.NotFound);

			var ex = await Record.ExceptionAsync(() => link.UpdateShortCode(ShortCodeId, new(), TestContext.Current.CancellationToken));

			Assert.IsType<NotFoundException>(ex);
			Assert.Equal(HyphenSdkResources.Link_UpdateShortCode404, ex.Message);
		}

		[Theory]
		[InlineData(HttpStatusCode.InternalServerError)]
		[InlineData(HttpStatusCode.NotImplemented)]
		public async ValueTask ReturnsNon200(HttpStatusCode statusCode)
		{
			var link = new Link(httpClientFactory, logger, Options.Create(options));
			ReturnStatusCode(statusCode);

			var ex = await Record.ExceptionAsync(() => link.UpdateShortCode(ShortCodeId, new(), TestContext.Current.CancellationToken));

			var hscEx = Assert.IsType<HttpStatusCodeException>(ex);
			Assert.Equal(statusCode, hscEx.StatusCode);
			Assert.Equal(HyphenSdkResources.Http_StatusCodeError(statusCode), hscEx.Message);
		}
	}

	SpyHttpMessageHandler ReturnContent<T>(T content, HttpStatusCode statusCode = HttpStatusCode.OK)
	{
		var handler = new SpyHttpMessageHandler(_ => new HttpResponseMessage(statusCode) { Content = JsonContent.Create(content) });

		httpClientFactory.CreateClient(Arg.Any<string>()).Returns(new HttpClient(handler));
		return handler;
	}

	SpyHttpMessageHandler ReturnStatusCode(HttpStatusCode statusCode)
	{
		var handler = new SpyHttpMessageHandler(_ => new HttpResponseMessage(statusCode));

		httpClientFactory.CreateClient(Arg.Any<string>()).Returns(new HttpClient(handler));
		return handler;
	}
}
