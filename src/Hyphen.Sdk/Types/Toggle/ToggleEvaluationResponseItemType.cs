namespace Hyphen.Sdk;

[JsonConverter(typeof(JsonStringEnumConverter<ToggleEvaluationResponseItemType>))]
internal enum ToggleEvaluationResponseItemType
{
	Boolean,
	String,
	Number,
	Object,
}
