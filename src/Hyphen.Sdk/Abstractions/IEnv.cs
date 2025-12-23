using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Hyphen.Sdk;

/// <summary>
/// A Hyphen service which can read <c>.env</c> files, and then retrieve environment variables.
/// </summary>
public interface IEnv
{
	/// <summary>
	/// Gets a <see cref="bool"/> value from the given environment variable.
	/// </summary>
	/// <param name="name">The environment variable name</param>
	/// <param name="required">When <see langword="true"/> and the variable does not exist (or is in the wrong
	/// format), will throw; when <see langword="false"/>, will return <see langword="null"/> instead.</param>
	/// <returns>The value</returns>
	/// <exception cref="InvalidOperationException">Thrown when <paramref name="required"/> is true
	/// and the variable does not exist (or is in the wrong format).</exception>
	/// <remarks>
	/// Boolean environment variables can be any of the following:<br />
	/// <list type="bullet">
	/// <item><b>true</b>: <c>"true"</c>, <c>"on"</c>, <c>"yes"</c>, <c>"1"</c></item>
	/// <item><b>false</b>: <c>"false"</c>, <c>"off"</c>, <c>"no"</c>, <c>"0"</c></item>
	/// </list>
	/// Values are case-insensitive.
	/// </remarks>
	bool? GetBool(string name, [NotNullWhen(true)] bool required);

	/// <summary>
	/// Gets a <see cref="decimal"/> value from the given environment variable.
	/// </summary>
	/// <param name="name">The environment variable name</param>
	/// <param name="numberStyles">A bitwise combination of enumeration values that indicates the permitted format.</param>
	/// <param name="formatProvider">An object that supplies culture-specific parsing information.</param>
	/// <param name="required">When <see langword="true"/> and the variable does not exist (or is in the wrong
	/// format), will throw; when <see langword="false"/>, will return <see langword="null"/> instead.</param>
	/// <returns>The value</returns>
	/// <exception cref="InvalidOperationException">Thrown when <paramref name="required"/> is true
	/// and the variable does not exist (or is in the wrong format).</exception>
	decimal? GetDecimal(string name, NumberStyles numberStyles, IFormatProvider? formatProvider, [NotNullWhen(true)] bool required);

	/// <summary>
	/// Gets a <see cref="double"/> value from the given environment variable.
	/// </summary>
	/// <param name="name">The environment variable name</param>
	/// <param name="numberStyles">A bitwise combination of enumeration values that indicates the permitted format.</param>
	/// <param name="formatProvider">An object that supplies culture-specific parsing information.</param>
	/// <param name="required">When <see langword="true"/> and the variable does not exist (or is in the wrong
	/// format), will throw; when <see langword="false"/>, will return <see langword="null"/> instead.</param>
	/// <returns>The value</returns>
	/// <exception cref="InvalidOperationException">Thrown when <paramref name="required"/> is true
	/// and the variable does not exist (or is in the wrong format).</exception>
	double? GetDouble(string name, NumberStyles numberStyles, IFormatProvider? formatProvider, [NotNullWhen(true)] bool required);

	/// <summary>
	/// Gets an <see cref="int"/> value from the given environment variable.
	/// </summary>
	/// <param name="name">The environment variable name</param>
	/// <param name="numberStyles">A bitwise combination of enumeration values that indicates the permitted format.</param>
	/// <param name="formatProvider">An object that supplies culture-specific parsing information.</param>
	/// <param name="required">When <see langword="true"/> and the variable does not exist (or is in the wrong
	/// format), will throw; when <see langword="false"/>, will return <see langword="null"/> instead.</param>
	/// <returns>The value</returns>
	/// <exception cref="InvalidOperationException">Thrown when <paramref name="required"/> is true
	/// and the variable does not exist (or is in the wrong format).</exception>
	int? GetInt(string name, NumberStyles numberStyles, IFormatProvider? formatProvider, [NotNullWhen(true)] bool required);

	/// <summary>
	/// Gets a <see cref="long"/> value from the given environment variable.
	/// </summary>
	/// <param name="name">The environment variable name</param>
	/// <param name="numberStyles">A bitwise combination of enumeration values that indicates the permitted format.</param>
	/// <param name="formatProvider">An object that supplies culture-specific parsing information.</param>
	/// <param name="required">When <see langword="true"/> and the variable does not exist (or is in the wrong
	/// format), will throw; when <see langword="false"/>, will return <see langword="null"/> instead.</param>
	/// <returns>The value</returns>
	/// <exception cref="InvalidOperationException">Thrown when <paramref name="required"/> is true
	/// and the variable does not exist (or is in the wrong format).</exception>
	long? GetLong(string name, NumberStyles numberStyles, IFormatProvider? formatProvider, [NotNullWhen(true)] bool required);

	/// <summary>
	/// Gets a <see cref="string"/> value from the given environment variable.
	/// </summary>
	/// <param name="name">The environment variable name</param>
	/// <param name="required">When <see langword="true"/> and the variable does not exist, will throw;
	/// when <see langword="false"/>, will return <see langword="null"/> instead.</param>
	/// <returns>The value</returns>
	/// <exception cref="InvalidOperationException">Thrown when <paramref name="required"/> is true
	/// and the variable does not exist (or is in the wrong format).</exception>
	string? GetString(string name, [NotNullWhen(true)] bool required);
}
