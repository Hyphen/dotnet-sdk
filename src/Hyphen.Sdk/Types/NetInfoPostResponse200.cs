using System.Diagnostics.CodeAnalysis;

namespace Hyphen.Sdk;

internal class NetInfoPostResponse200
{
	[JsonPropertyName("data")]
	[ExcludeFromCodeCoverage]
	public required NetInfoResult[] Data { get; set; }
}
