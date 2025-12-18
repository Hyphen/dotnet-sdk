using System.Diagnostics.CodeAnalysis;

namespace Hyphen.Sdk;

[ExcludeFromCodeCoverage]
internal class NetInfoPostResponse200
{
	[JsonPropertyName("data")]
	public required NetInfoResult[] Data { get; set; }
}
