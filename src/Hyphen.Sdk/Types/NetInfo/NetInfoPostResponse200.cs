namespace Hyphen.Sdk;

internal class NetInfoPostResponse200
{
	[JsonPropertyName("data")]
	public required NetInfoResult[] Data { get; set; }
}
