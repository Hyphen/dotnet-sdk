using System.Diagnostics.CodeAnalysis;

namespace Hyphen.Sdk;

/// <summary>
/// Hyphen SDK extensions for <see cref="IToggle"/>.
/// </summary>
[ExcludeFromCodeCoverage]
public static class HyphenSdkToggleExtensions
{
	/// <summary>
	/// Evaluates a toggle.
	/// </summary>
	/// <typeparam name="T">The type of the toggle result.</typeparam>
	/// <param name="toggle"/>
	/// <param name="toggleKey">The key of the toggle to retrieve.</param>
	/// <returns>The evaluation of the toggle.</returns>
	/// <exception cref="InvalidOperationException">Thrown if the response does not deserialize into <typeparamref name="T"/>.</exception>
	public static Task<ToggleEvaluation<T?>> Evaluate<T>(this IToggle toggle, string toggleKey) =>
		Guard.ArgumentNotNull(toggle).Evaluate<T>(toggleKey, default, default, default);

	/// <summary>
	/// Evaluates a toggle.
	/// </summary>
	/// <typeparam name="T">The type of the toggle result.</typeparam>
	/// <param name="toggle"/>
	/// <param name="toggleKey">The key of the toggle to retrieve.</param>
	/// <param name="cancellationToken">The cancellation token to cancel the request early.</param>
	/// <returns>The evaluation of the toggle.</returns>
	/// <exception cref="InvalidOperationException">Thrown if the response does not deserialize into <typeparamref name="T"/>.</exception>
	public static Task<ToggleEvaluation<T?>> Evaluate<T>(this IToggle toggle, string toggleKey, CancellationToken cancellationToken) =>
		Guard.ArgumentNotNull(toggle).Evaluate<T>(toggleKey, default, default, cancellationToken);

	/// <summary>
	/// Evaluates a toggle.
	/// </summary>
	/// <typeparam name="T">The type of the toggle result.</typeparam>
	/// <param name="toggle"/>
	/// <param name="toggleKey">The key of the toggle to retrieve.</param>
	/// <param name="defaultValue">The default value to return if the toggle is not found or an error occurs.</param>
	/// <returns>The evaluation of the toggle.</returns>
	/// <exception cref="InvalidOperationException">Thrown if the response does not deserialize into <typeparamref name="T"/>.</exception>
	public static Task<ToggleEvaluation<T?>> Evaluate<T>(this IToggle toggle, string toggleKey, T? defaultValue) =>
		Guard.ArgumentNotNull(toggle).Evaluate<T>(toggleKey, defaultValue, default, default);

	/// <summary>
	/// Evaluates a toggle.
	/// </summary>
	/// <typeparam name="T">The type of the toggle result.</typeparam>
	/// <param name="toggle"/>
	/// <param name="toggleKey">The key of the toggle to retrieve.</param>
	/// <param name="defaultValue">The default value to return if the toggle is not found or an error occurs.</param>
	/// <param name="cancellationToken">The cancellation token to cancel the request early.</param>
	/// <returns>The evaluation of the toggle.</returns>
	/// <exception cref="InvalidOperationException">Thrown if the response does not deserialize into <typeparamref name="T"/>.</exception>
	public static Task<ToggleEvaluation<T?>> Evaluate<T>(this IToggle toggle, string toggleKey, T? defaultValue, CancellationToken cancellationToken) =>
		Guard.ArgumentNotNull(toggle).Evaluate<T>(toggleKey, defaultValue, default, cancellationToken);

	/// <summary>
	/// Evaluates a toggle.
	/// </summary>
	/// <typeparam name="T">The type of the toggle result.</typeparam>
	/// <param name="toggle"/>
	/// <param name="toggleKey">The key of the toggle to retrieve.</param>
	/// <param name="parms">The optional parameters for evaluating the toggle.</param>
	/// <returns>The evaluation of the toggle.</returns>
	/// <exception cref="InvalidOperationException">Thrown if the response does not deserialize into <typeparamref name="T"/>.</exception>
	public static Task<ToggleEvaluation<T?>> Evaluate<T>(this IToggle toggle, string toggleKey, EvaluateParams? parms) =>
		Guard.ArgumentNotNull(toggle).Evaluate<T>(toggleKey, default, parms, default);

	/// <summary>
	/// Evaluates a toggle.
	/// </summary>
	/// <typeparam name="T">The type of the toggle result.</typeparam>
	/// <param name="toggle"/>
	/// <param name="toggleKey">The key of the toggle to retrieve.</param>
	/// <param name="parms">The optional parameters for evaluating the toggle.</param>
	/// <param name="cancellationToken">The cancellation token to cancel the request early.</param>
	/// <returns>The evaluation of the toggle.</returns>
	/// <exception cref="InvalidOperationException">Thrown if the response does not deserialize into <typeparamref name="T"/>.</exception>
	public static Task<ToggleEvaluation<T?>> Evaluate<T>(this IToggle toggle, string toggleKey, EvaluateParams? parms, CancellationToken cancellationToken) =>
		Guard.ArgumentNotNull(toggle).Evaluate<T>(toggleKey, default, parms, cancellationToken);

	/// <summary>
	/// Evaluates a toggle.
	/// </summary>
	/// <typeparam name="T">The type of the toggle result.</typeparam>
	/// <param name="toggle"/>
	/// <param name="toggleKey">The key of the toggle to retrieve.</param>
	/// <param name="defaultValue">The default value to return if the toggle is not found or an error occurs.</param>
	/// <param name="parms">The optional parameters for evaluating the toggle.</param>
	/// <returns>The evaluation of the toggle.</returns>
	/// <exception cref="InvalidOperationException">Thrown if the response does not deserialize into <typeparamref name="T"/>.</exception>
	public static Task<ToggleEvaluation<T?>> Evaluate<T>(this IToggle toggle, string toggleKey, T? defaultValue, EvaluateParams? parms) =>
		Guard.ArgumentNotNull(toggle).Evaluate<T>(toggleKey, defaultValue, parms, default);
}
