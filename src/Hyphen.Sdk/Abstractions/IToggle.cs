namespace Hyphen.Sdk;

/// <summary>
/// A Hyphen service which can request feature flag information.
/// </summary>
public interface IToggle
{
	/// <summary>
	/// Evaluates a toggle.
	/// </summary>
	/// <typeparam name="T">The type of the toggle result.</typeparam>
	/// <param name="toggleKey">The key of the toggle to retrieve.</param>
	/// <param name="defaultValue">The default value to return if the toggle is not found or an error occurs.</param>
	/// <param name="parms">The optional parameters for evaluating the toggle.</param>
	/// <param name="cancellationToken">The cancellation token to cancel the request early.</param>
	/// <returns>The evaluation of the toggle.</returns>
	/// <exception cref="InvalidOperationException">Thrown if the response does not deserialize into <typeparamref name="T"/>.</exception>
	Task<ToggleEvaluation<T?>> Evaluate<T>(string toggleKey, T? defaultValue, EvaluateParams? parms, CancellationToken cancellationToken);
}
