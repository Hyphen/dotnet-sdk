using System.Diagnostics.CodeAnalysis;

namespace Hyphen.Sdk;

/// <summary>
/// Hyphen SDK extensions for <see cref="INetInfo"/>.
/// </summary>
[ExcludeFromCodeCoverage]
public static class HyphenSdkNetInfoExtensions
{
	/// <summary>
	/// Gets information about the current computer's public IP address.
	/// </summary>
	/// <param name="netInfo"/>
	/// <returns>
	/// The return value will be populated with information about the IP address if a lookup
	/// was successful, or error information if the lookup was unsuccessful.
	/// </returns>
	/// <remarks>
	/// This API supports both IPv4 and IPv6 addresses.
	/// </remarks>
	public static async Task<NetInfoResult> GetIPInfo(this INetInfo netInfo) =>
		(await Guard.ArgumentNotNull(netInfo).GetIPInfos([], default).ConfigureAwait(false))[0];

	/// <summary>
	/// Gets information about the current computer's public IP address.
	/// </summary>
	/// <param name="netInfo"/>
	/// <param name="cancellationToken">The cancellation token to cancel the request early</param>
	/// <returns>
	/// The return value will be populated with information about the IP address if a lookup
	/// was successful, or error information if the lookup was unsuccessful.
	/// </returns>
	/// <remarks>
	/// This API supports both IPv4 and IPv6 addresses.
	/// </remarks>
	public static async Task<NetInfoResult> GetIPInfo(this INetInfo netInfo, CancellationToken cancellationToken) =>
		(await Guard.ArgumentNotNull(netInfo).GetIPInfos([], cancellationToken).ConfigureAwait(false))[0];

	/// <summary>
	/// Gets information about a single IP address.
	/// </summary>
	/// <param name="netInfo"/>
	/// <param name="ip">The IP address</param>
	/// <returns>
	/// The return value will be populated with information about the IP address if a lookup
	/// was successful, or error information if the lookup was unsuccessful.
	/// </returns>
	/// <remarks>
	/// This API supports both IPv4 and IPv6 addresses.
	/// </remarks>
	public static async Task<NetInfoResult> GetIPInfo(this INetInfo netInfo, string ip) =>
		(await Guard.ArgumentNotNull(netInfo).GetIPInfos([ip], default).ConfigureAwait(false))[0];

	/// <summary>
	/// Gets information about a single IP address.
	/// </summary>
	/// <param name="netInfo"/>
	/// <param name="ip">The IP address</param>
	/// <param name="cancellationToken">The cancellation token to cancel the request early</param>
	/// <returns>
	/// The return value will be populated with information about the IP address if a lookup
	/// was successful, or error information if the lookup was unsuccessful.
	/// </returns>
	/// <remarks>
	/// This API supports both IPv4 and IPv6 addresses.
	/// </remarks>
	public static async Task<NetInfoResult> GetIPInfo(this INetInfo netInfo, string ip, CancellationToken cancellationToken) =>
		(await Guard.ArgumentNotNull(netInfo).GetIPInfos([ip], cancellationToken).ConfigureAwait(false))[0];

	/// <summary>
	/// Gets information about one or more IP address.
	/// </summary>
	/// <param name="netInfo"/>
	/// <param name="ips">The IP addresses</param>
	/// <returns>
	/// The return value will be populated with information about the IP address if a lookup
	/// was successful, or error information if the lookup was unsuccessful.
	/// </returns>
	/// <remarks>
	/// This API supports both IPv4 and IPv6 addresses.
	/// </remarks>
	public static Task<NetInfoResult[]> GetIPInfos(this INetInfo netInfo, params string[] ips) =>
		Guard.ArgumentNotNull(netInfo).GetIPInfos(ips, default);
}
