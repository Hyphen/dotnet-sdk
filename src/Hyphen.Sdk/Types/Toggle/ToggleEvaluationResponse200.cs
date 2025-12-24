namespace Hyphen.Sdk;

internal class ToggleEvaluationResponse200
{
	[JsonPropertyName("toggles")]
	public required IReadOnlyDictionary<string, ToggleEvaluationResponseItem> Toggles { get; set; }
}
