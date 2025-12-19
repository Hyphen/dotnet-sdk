using System.Net.Http.Headers;

namespace System.Net.Http;

internal class SpyHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> handler) : HttpMessageHandler
{
	public List<(HttpMethod Method, HttpRequestHeaders Headers, string? Uri, string? Body)> Requests = [];

	protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		var content =
			request.Content is not null
				? await request.Content.ReadAsStringAsync(cancellationToken)
				: null;

		Requests.Add((request.Method, request.Headers, request.RequestUri?.ToString(), content));
		return handler(request);
	}
}
