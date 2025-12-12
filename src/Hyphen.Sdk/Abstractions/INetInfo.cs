namespace Hyphen.Sdk;

/// <summary>
/// A Hyphen service which can request information about IP addresses.
/// </summary>
public interface INetInfo
{
	/// <summary>
	/// Gets information about one or more IP address.
	/// </summary>
	/// <param name="ips">The IP addresses</param>
	/// <param name="cancellationToken">The cancellation token to cancel the request early</param>
	/// <returns>
	/// The return value will be populated with information about the IP address if a lookup
	/// was successful, or error information if the lookup was unsuccessful.
	/// </returns>
	/// <remarks>
	/// This API supports both IPv4 and IPv6 addresses.
	/// </remarks>
	ValueTask<NetInfoResult[]> GetIPInfos(string[] ips, CancellationToken cancellationToken);
}
