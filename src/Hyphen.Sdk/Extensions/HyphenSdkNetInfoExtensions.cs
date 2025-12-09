using System.Diagnostics.CodeAnalysis;

namespace Hyphen.Sdk;

/// <summary>
/// Extension methods for <see cref="INetInfo"/>.
/// </summary>
[SuppressMessage("Design", "CA1034:Nested types should not be visible", Justification = "Extension blocks are implemented as nested types")]
public static class HyphenSdkNetInfoExtensions
{
	extension(INetInfo netInfo)
	{
		/// <summary>
		/// Gets information about a single IP address.
		/// </summary>
		/// <param name="ip">The IP address</param>
		/// <returns>
		/// The return value will be populated with information about the IP address if a lookup
		/// was successful, or error information if the lookup was unsuccessful.
		/// </returns>
		/// <remarks>
		/// This API supports both IPv4 and IPv6 addresses.
		/// </remarks>
		public async ValueTask<NetInfoResult> GetIPInfo(string ip) =>
			(await netInfo.GetIPInfos([ip], default).ConfigureAwait(false))[0];

		/// <summary>
		/// Gets information about a single IP address.
		/// </summary>
		/// <param name="ip">The IP address</param>
		/// <param name="cancellationToken">The cancellation token to cancel the request early</param>
		/// <returns>
		/// The return value will be populated with information about the IP address if a lookup
		/// was successful, or error information if the lookup was unsuccessful.
		/// </returns>
		/// <remarks>
		/// This API supports both IPv4 and IPv6 addresses.
		/// </remarks>
		public async ValueTask<NetInfoResult> GetIPInfo(string ip, CancellationToken cancellationToken) =>
			(await netInfo.GetIPInfos([ip], cancellationToken).ConfigureAwait(false))[0];

		/// <summary>
		/// Gets information about one or more IP address.
		/// </summary>
		/// <param name="ips">The IP addresses</param>
		/// <returns>
		/// The return value will be populated with information about the IP address if a lookup
		/// was successful, or error information if the lookup was unsuccessful.
		/// </returns>
		/// <remarks>
		/// This API supports both IPv4 and IPv6 addresses.
		/// </remarks>
		public ValueTask<NetInfoResult[]> GetIPInfos(params string[] ips) =>
			netInfo.GetIPInfos(ips, default);
	}
}
