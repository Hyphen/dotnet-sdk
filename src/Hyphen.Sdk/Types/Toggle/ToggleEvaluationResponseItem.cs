namespace Hyphen.Sdk;

internal class ToggleEvaluationResponseItem
{
	[JsonPropertyName("key")]
	public required string Key { get; set; }

	[JsonPropertyName("reason")]
	public string? Reason { get; set; }

	[JsonPropertyName("type")]
	public ToggleEvaluationResponseItemType Type { get; set; }

	[JsonPropertyName("value")]
	public JsonElement? Value { get; set; }
}
