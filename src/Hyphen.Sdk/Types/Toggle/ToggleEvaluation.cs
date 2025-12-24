namespace Hyphen.Sdk;

/// <summary>
/// Represents the result of a toggle evaluation.
/// </summary>
public class ToggleEvaluation<T>
{
	/// <summary>
	/// Gets or sets the exception that was thrown during evaluation.
	/// </summary>
	public Exception? Exception { get; set; }

	/// <summary>
	/// Gets or sets the key for the evaluation.
	/// </summary>
	public required string Key { get; set; }

	/// <summary>
	/// Gets the reason for the evaluation result.
	/// </summary>
	public string? Reason { get; set; }

	/// <summary>
	/// Gets the value of the evaluation.
	/// </summary>
	public required T? Value { get; set; }
}
