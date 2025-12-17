using System.Diagnostics.CodeAnalysis;

namespace Hyphen.Sdk;

/// <summary>
/// Represents the options values that can be used to configure <see cref="HyphenSdkServiceCollectionExtensions.AddHyphenEnv"/>.
/// </summary>
[ExcludeFromCodeCoverage]
public class EnvOptions
{
	/// <summary>
	/// Gets or sets the environment name to load. Causes <c>.env.{environment}</c> to be loaded
	/// in addition to <c>.env</c>.
	/// </summary>
	/// <remarks>
	/// If this value remains unset, and environment variable <c>HYPHEN_APP_ENVIRONMENT</c> is
	/// set, then the value from the environment variable will be used instead. If there is
	/// no environment variable set for the environment, then no environment-specific files
	/// will be loaded.
	/// </remarks>
	public string? Environment { get; set; }

	/// <summary>
	/// Gets or sets a flag which indicates whether to load <c>.local</c> files. If true, causes
	/// <c>.env.local</c> (and <c>.env.{environment}.local</c>, if <see cref="Environment"/> is
	/// set) to be loaded.
	/// </summary>
	/// <remarks>
	/// If this value remains unset, then <c>true</c> will be used.
	/// </remarks>
	public bool? Local { get; set; } = true;

	/// <summary>
	/// Gets or sets the path to load the <c>.env</c> files from.
	/// </summary>
	/// <remarks>
	/// If this value remains unset, then <see cref="AppContext.BaseDirectory"/> is used.
	/// </remarks>
	public string? Path { get; set; }

	// Internal testing hooks

	internal Func<string, string?> GetEnv { get; set; } = System.Environment.GetEnvironmentVariable;

	internal Func<string, string[]?> ReadFile { get; set; } = path => File.Exists(path) ? File.ReadAllLines(path) : null;

	internal Action<string, string?> SetEnv { get; set; } = System.Environment.SetEnvironmentVariable;
}
