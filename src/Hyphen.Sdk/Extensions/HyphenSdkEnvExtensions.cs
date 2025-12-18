using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Hyphen.Sdk.Internal;

namespace Hyphen.Sdk;

/// <summary>
/// Hyphen SDK extensions for <see cref="IEnv"/>.
/// </summary>
[ExcludeFromCodeCoverage]
public static class HyphenSdkEnvExtensions
{
	/// <summary>
	/// Gets an optional <see cref="bool"/> value from the given environment variable.
	/// </summary>
	/// <param name="env"/>
	/// <param name="name">The environment variable name</param>
	/// <returns>The value</returns>
	public static bool? GetBool(this IEnv env, string name) =>
		Guard.ArgumentNotNull(env).GetBool(name, required: false);

	/// <summary>
	/// Gets an optional <see cref="decimal"/> value from the given environment variable.
	/// </summary>
	/// <param name="env"/>
	/// <param name="name">The environment variable name</param>
	/// <returns>The value</returns>
	public static decimal? GetDecimal(this IEnv env, string name) =>
		Guard.ArgumentNotNull(env).GetDecimal(name, NumberStyles.Number, formatProvider: null, required: false);

	/// <summary>
	/// Gets a <see cref="decimal"/> value from the given environment variable.
	/// </summary>
	/// <param name="env"/>
	/// <param name="name">The environment variable name</param>
	/// <param name="required">When <c>true</c> and the variable does not exist (or is in the wrong
	/// format), will throw; when <c>false</c>, will return <c>null</c> instead.</param>
	/// <returns>The value</returns>
	/// <exception cref="InvalidOperationException">Thrown when <paramref name="required"/> is true
	/// and the variable does not exist (or is in the wrong format).</exception>
	public static decimal? GetDecimal(this IEnv env, string name, [NotNullWhen(true)] bool required) =>
		Guard.ArgumentNotNull(env).GetDecimal(name, NumberStyles.Number, formatProvider: null, required);

	/// <summary>
	/// Gets an optional <see cref="decimal"/> value from the given environment variable.
	/// </summary>
	/// <param name="env"/>
	/// <param name="name">The environment variable name</param>
	/// <param name="formatProvider">An object that supplies culture-specific parsing information.</param>
	/// <returns>The value</returns>
	public static decimal? GetDecimal(this IEnv env, string name, IFormatProvider? formatProvider) =>
		Guard.ArgumentNotNull(env).GetDecimal(name, NumberStyles.Number, formatProvider, required: false);

	/// <summary>
	/// Gets a <see cref="decimal"/> value from the given environment variable.
	/// </summary>
	/// <param name="env"/>
	/// <param name="name">The environment variable name</param>
	/// <param name="formatProvider">An object that supplies culture-specific parsing information.</param>
	/// <param name="required">When <c>true</c> and the variable does not exist (or is in the wrong
	/// format), will throw; when <c>false</c>, will return <c>null</c> instead.</param>
	/// <returns>The value</returns>
	/// <exception cref="InvalidOperationException">Thrown when <paramref name="required"/> is true
	/// and the variable does not exist (or is in the wrong format).</exception>
	public static decimal? GetDecimal(this IEnv env, string name, IFormatProvider? formatProvider, [NotNullWhen(true)] bool required) =>
		Guard.ArgumentNotNull(env).GetDecimal(name, NumberStyles.Number, formatProvider, required);

	/// <summary>
	/// Gets an optional <see cref="decimal"/> value from the given environment variable.
	/// </summary>
	/// <param name="env"/>
	/// <param name="name">The environment variable name</param>
	/// <param name="numberStyles">A bitwise combination of enumeration values that indicates the permitted format.</param>
	/// <returns>The value</returns>
	public static decimal? GetDecimal(this IEnv env, string name, NumberStyles numberStyles) =>
		Guard.ArgumentNotNull(env).GetDecimal(name, numberStyles, formatProvider: null, required: false);

	/// <summary>
	/// Gets a <see cref="decimal"/> value from the given environment variable.
	/// </summary>
	/// <param name="env"/>
	/// <param name="name">The environment variable name</param>
	/// <param name="numberStyles">A bitwise combination of enumeration values that indicates the permitted format.</param>
	/// <param name="required">When <c>true</c> and the variable does not exist (or is in the wrong
	/// format), will throw; when <c>false</c>, will return <c>null</c> instead.</param>
	/// <returns>The value</returns>
	/// <exception cref="InvalidOperationException">Thrown when <paramref name="required"/> is true
	/// and the variable does not exist (or is in the wrong format).</exception>
	public static decimal? GetDecimal(this IEnv env, string name, NumberStyles numberStyles, [NotNullWhen(true)] bool required) =>
		Guard.ArgumentNotNull(env).GetDecimal(name, numberStyles, formatProvider: null, required);

	/// <summary>
	/// Gets an optional <see cref="double"/> value from the given environment variable.
	/// </summary>
	/// <param name="env"/>
	/// <param name="name">The environment variable name</param>
	/// <returns>The value</returns>
	public static double? GetDouble(this IEnv env, string name) =>
		Guard.ArgumentNotNull(env).GetDouble(name, NumberStyles.Number, formatProvider: null, required: false);

	/// <summary>
	/// Gets a <see cref="double"/> value from the given environment variable.
	/// </summary>
	/// <param name="env"/>
	/// <param name="name">The environment variable name</param>
	/// <param name="required">When <c>true</c> and the variable does not exist (or is in the wrong
	/// format), will throw; when <c>false</c>, will return <c>null</c> instead.</param>
	/// <returns>The value</returns>
	/// <exception cref="InvalidOperationException">Thrown when <paramref name="required"/> is true
	/// and the variable does not exist (or is in the wrong format).</exception>
	public static double? GetDouble(this IEnv env, string name, [NotNullWhen(true)] bool required) =>
		Guard.ArgumentNotNull(env).GetDouble(name, NumberStyles.Number, formatProvider: null, required);

	/// <summary>
	/// Gets an optional <see cref="double"/> value from the given environment variable.
	/// </summary>
	/// <param name="env"/>
	/// <param name="name">The environment variable name</param>
	/// <param name="formatProvider">An object that supplies culture-specific parsing information.</param>
	/// <returns>The value</returns>
	public static double? GetDouble(this IEnv env, string name, IFormatProvider? formatProvider) =>
		Guard.ArgumentNotNull(env).GetDouble(name, NumberStyles.Number, formatProvider, required: false);

	/// <summary>
	/// Gets a <see cref="double"/> value from the given environment variable.
	/// </summary>
	/// <param name="env"/>
	/// <param name="name">The environment variable name</param>
	/// <param name="formatProvider">An object that supplies culture-specific parsing information.</param>
	/// <param name="required">When <c>true</c> and the variable does not exist (or is in the wrong
	/// format), will throw; when <c>false</c>, will return <c>null</c> instead.</param>
	/// <returns>The value</returns>
	/// <exception cref="InvalidOperationException">Thrown when <paramref name="required"/> is true
	/// and the variable does not exist (or is in the wrong format).</exception>
	public static double? GetDouble(this IEnv env, string name, IFormatProvider? formatProvider, [NotNullWhen(true)] bool required) =>
		Guard.ArgumentNotNull(env).GetDouble(name, NumberStyles.Number, formatProvider, required);

	/// <summary>
	/// Gets an optional <see cref="double"/> value from the given environment variable.
	/// </summary>
	/// <param name="env"/>
	/// <param name="name">The environment variable name</param>
	/// <param name="numberStyles">A bitwise combination of enumeration values that indicates the permitted format.</param>
	/// <returns>The value</returns>
	public static double? GetDouble(this IEnv env, string name, NumberStyles numberStyles) =>
		Guard.ArgumentNotNull(env).GetDouble(name, numberStyles, formatProvider: null, required: false);

	/// <summary>
	/// Gets a <see cref="double"/> value from the given environment variable.
	/// </summary>
	/// <param name="env"/>
	/// <param name="name">The environment variable name</param>
	/// <param name="numberStyles">A bitwise combination of enumeration values that indicates the permitted format.</param>
	/// <param name="required">When <c>true</c> and the variable does not exist (or is in the wrong
	/// format), will throw; when <c>false</c>, will return <c>null</c> instead.</param>
	/// <returns>The value</returns>
	/// <exception cref="InvalidOperationException">Thrown when <paramref name="required"/> is true
	/// and the variable does not exist (or is in the wrong format).</exception>
	public static double? GetDouble(this IEnv env, string name, NumberStyles numberStyles, [NotNullWhen(true)] bool required) =>
		Guard.ArgumentNotNull(env).GetDouble(name, numberStyles, formatProvider: null, required);

	/// <summary>
	/// Gets an optional <see cref="int"/> value from the given environment variable.
	/// </summary>
	/// <param name="env"/>
	/// <param name="name">The environment variable name</param>
	/// <returns>The value</returns>
	public static int? GetInt(this IEnv env, string name) =>
		Guard.ArgumentNotNull(env).GetInt(name, NumberStyles.Number, formatProvider: null, required: false);

	/// <summary>
	/// Gets a <see cref="int"/> value from the given environment variable.
	/// </summary>
	/// <param name="env"/>
	/// <param name="name">The environment variable name</param>
	/// <param name="required">When <c>true</c> and the variable does not exist (or is in the wrong
	/// format), will throw; when <c>false</c>, will return <c>null</c> instead.</param>
	/// <returns>The value</returns>
	/// <exception cref="InvalidOperationException">Thrown when <paramref name="required"/> is true
	/// and the variable does not exist (or is in the wrong format).</exception>
	public static int? GetInt(this IEnv env, string name, [NotNullWhen(true)] bool required) =>
		Guard.ArgumentNotNull(env).GetInt(name, NumberStyles.Number, formatProvider: null, required);

	/// <summary>
	/// Gets an optional <see cref="int"/> value from the given environment variable.
	/// </summary>
	/// <param name="env"/>
	/// <param name="name">The environment variable name</param>
	/// <param name="formatProvider">An object that supplies culture-specific parsing information.</param>
	/// <returns>The value</returns>
	public static int? GetInt(this IEnv env, string name, IFormatProvider? formatProvider) =>
		Guard.ArgumentNotNull(env).GetInt(name, NumberStyles.Number, formatProvider, required: false);

	/// <summary>
	/// Gets a <see cref="int"/> value from the given environment variable.
	/// </summary>
	/// <param name="env"/>
	/// <param name="name">The environment variable name</param>
	/// <param name="formatProvider">An object that supplies culture-specific parsing information.</param>
	/// <param name="required">When <c>true</c> and the variable does not exist (or is in the wrong
	/// format), will throw; when <c>false</c>, will return <c>null</c> instead.</param>
	/// <returns>The value</returns>
	/// <exception cref="InvalidOperationException">Thrown when <paramref name="required"/> is true
	/// and the variable does not exist (or is in the wrong format).</exception>
	public static int? GetInt(this IEnv env, string name, IFormatProvider? formatProvider, [NotNullWhen(true)] bool required) =>
		Guard.ArgumentNotNull(env).GetInt(name, NumberStyles.Number, formatProvider, required);

	/// <summary>
	/// Gets an optional <see cref="int"/> value from the given environment variable.
	/// </summary>
	/// <param name="env"/>
	/// <param name="name">The environment variable name</param>
	/// <param name="numberStyles">A bitwise combination of enumeration values that indicates the permitted format.</param>
	/// <returns>The value</returns>
	public static int? GetInt(this IEnv env, string name, NumberStyles numberStyles) =>
		Guard.ArgumentNotNull(env).GetInt(name, numberStyles, formatProvider: null, required: false);

	/// <summary>
	/// Gets a <see cref="int"/> value from the given environment variable.
	/// </summary>
	/// <param name="env"/>
	/// <param name="name">The environment variable name</param>
	/// <param name="numberStyles">A bitwise combination of enumeration values that indicates the permitted format.</param>
	/// <param name="required">When <c>true</c> and the variable does not exist (or is in the wrong
	/// format), will throw; when <c>false</c>, will return <c>null</c> instead.</param>
	/// <returns>The value</returns>
	/// <exception cref="InvalidOperationException">Thrown when <paramref name="required"/> is true
	/// and the variable does not exist (or is in the wrong format).</exception>
	public static int? GetInt(this IEnv env, string name, NumberStyles numberStyles, [NotNullWhen(true)] bool required) =>
		Guard.ArgumentNotNull(env).GetInt(name, numberStyles, formatProvider: null, required);

	/// <summary>
	/// Gets an optional <see cref="long"/> value from the given environment variable.
	/// </summary>
	/// <param name="env"/>
	/// <param name="name">The environment variable name</param>
	/// <returns>The value</returns>
	public static long? GetLong(this IEnv env, string name) =>
		Guard.ArgumentNotNull(env).GetLong(name, NumberStyles.Number, formatProvider: null, required: false);

	/// <summary>
	/// Gets a <see cref="long"/> value from the given environment variable.
	/// </summary>
	/// <param name="env"/>
	/// <param name="name">The environment variable name</param>
	/// <param name="required">When <c>true</c> and the variable does not exist (or is in the wrong
	/// format), will throw; when <c>false</c>, will return <c>null</c> instead.</param>
	/// <returns>The value</returns>
	/// <exception cref="InvalidOperationException">Thrown when <paramref name="required"/> is true
	/// and the variable does not exist (or is in the wrong format).</exception>
	public static long? GetLong(this IEnv env, string name, [NotNullWhen(true)] bool required) =>
		Guard.ArgumentNotNull(env).GetLong(name, NumberStyles.Number, formatProvider: null, required);

	/// <summary>
	/// Gets an optional <see cref="long"/> value from the given environment variable.
	/// </summary>
	/// <param name="env"/>
	/// <param name="name">The environment variable name</param>
	/// <param name="formatProvider">An object that supplies culture-specific parsing information.</param>
	/// <returns>The value</returns>
	public static long? GetLong(this IEnv env, string name, IFormatProvider? formatProvider) =>
		Guard.ArgumentNotNull(env).GetLong(name, NumberStyles.Number, formatProvider, required: false);

	/// <summary>
	/// Gets a <see cref="long"/> value from the given environment variable.
	/// </summary>
	/// <param name="env"/>
	/// <param name="name">The environment variable name</param>
	/// <param name="formatProvider">An object that supplies culture-specific parsing information.</param>
	/// <param name="required">When <c>true</c> and the variable does not exist (or is in the wrong
	/// format), will throw; when <c>false</c>, will return <c>null</c> instead.</param>
	/// <returns>The value</returns>
	/// <exception cref="InvalidOperationException">Thrown when <paramref name="required"/> is true
	/// and the variable does not exist (or is in the wrong format).</exception>
	public static long? GetLong(this IEnv env, string name, IFormatProvider? formatProvider, [NotNullWhen(true)] bool required) =>
		Guard.ArgumentNotNull(env).GetLong(name, NumberStyles.Number, formatProvider, required);

	/// <summary>
	/// Gets an optional <see cref="long"/> value from the given environment variable.
	/// </summary>
	/// <param name="env"/>
	/// <param name="name">The environment variable name</param>
	/// <param name="numberStyles">A bitwise combination of enumeration values that indicates the permitted format.</param>
	/// <returns>The value</returns>
	public static long? GetLong(this IEnv env, string name, NumberStyles numberStyles) =>
		Guard.ArgumentNotNull(env).GetLong(name, numberStyles, formatProvider: null, required: false);

	/// <summary>
	/// Gets a <see cref="long"/> value from the given environment variable.
	/// </summary>
	/// <param name="env"/>
	/// <param name="name">The environment variable name</param>
	/// <param name="numberStyles">A bitwise combination of enumeration values that indicates the permitted format.</param>
	/// <param name="required">When <c>true</c> and the variable does not exist (or is in the wrong
	/// format), will throw; when <c>false</c>, will return <c>null</c> instead.</param>
	/// <returns>The value</returns>
	/// <exception cref="InvalidOperationException">Thrown when <paramref name="required"/> is true
	/// and the variable does not exist (or is in the wrong format).</exception>
	public static long? GetLong(this IEnv env, string name, NumberStyles numberStyles, [NotNullWhen(true)] bool required) =>
		Guard.ArgumentNotNull(env).GetLong(name, numberStyles, formatProvider: null, required);

	/// <summary>
	/// Gets an optional <see cref="string"/> value from the given environment variable.
	/// </summary>
	/// <param name="env"/>
	/// <param name="name">The environment variable name</param>
	/// <returns>The value</returns>
	public static string? GetString(this IEnv env, string name) =>
		Guard.ArgumentNotNull(env).GetString(name, required: false);
}
