namespace Hyphen.Sdk;

internal class ToggleEvaluationContext : ToggleContext
{
	[JsonPropertyName("application")]
	public required string Application { get; set; }

	[JsonPropertyName("environment")]
	public required string Environment { get; set; }
}
