namespace Hyphen.Sdk;

/// <summary>
/// Represents optional parameters for <see cref="IToggle.Evaluate"/>.
/// </summary>
public class EvaluateParams
{
	/// <summary>
	/// Gets or sets a flag indicating whether to cache the response (if caching is configured).
	/// </summary>
	public bool? Cache { get; set; }

	/// <summary>
	/// Gets or sets the toggle context used to inform the evaluation.
	/// </summary>
	public ToggleContext? Context { get; set; }
}
