using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;
using Hyphen.Sdk.Internal;
using Hyphen.Sdk.Resources;

namespace Hyphen.Sdk;

// The implementation of this class is modeled after https://github.com/motdotla/dotenv since this is the library
// that is currently used in the Hyphen JavaScript/TypeScript SDKs.

internal partial class Env : BaseService, IEnv
{
	public const string ApiKey = "HYPHEN_API_KEY";
	public const string AppEnvironment = "HYPHEN_APP_ENVIRONMENT";
	public const string Dev = "HYPHEN_DEV";

	const string NameRegexPattern =
		/* lang=regex */ @"^([a-zA-Z_]+[a-zA-Z0-9_]*)\s*=\s*(.*?)$";
	const string TerminationBackQuoteRegexPattern =
		/* lang=regex */ @"^(.*?)`\s*(#.*)?$";
	const string TerminationDoubleQuoteRegexPattern =
		/* lang=regex */ @"^(.*?)""\s*(#.*)?$";
	const string TerminationSingleQuoteRegexPattern =
		/* lang=regex */ @"^(.*?)'\s*(#.*)?$";
	const string ValueRegexPattern =
		/* lang=regex */ @"^(`(.*?)(`(\s*#.*)?)?|""(.*?)(""(\s*#.*)?)?|'(.*?)('(\s*#.*)?)?|([^`""'].*?)(\s*#.*)?|)$";

#if NETSTANDARD
	static readonly Regex nameRegex = new(NameRegexPattern, RegexOptions.Compiled);
	static readonly Regex terminationBackQuoteRegex = new(TerminationBackQuoteRegexPattern, RegexOptions.Compiled);
	static readonly Regex terminationDoubleQuoteRegex = new(TerminationDoubleQuoteRegexPattern, RegexOptions.Compiled);
	static readonly Regex terminationSingleQuoteRegex = new(TerminationSingleQuoteRegexPattern, RegexOptions.Compiled);
	static readonly Regex valueRegex = new(ValueRegexPattern, RegexOptions.Compiled);
#else
	static readonly Regex nameRegex = generateNameRegex();
	static readonly Regex terminationBackQuoteRegex = generateTerminationBackQuoteRegex();
	static readonly Regex terminationDoubleQuoteRegex = generateTerminationDoubleQuoteRegex();
	static readonly Regex terminationSingleQuoteRegex = generateTerminationSingleQuoteRegex();
	static readonly Regex valueRegex = generateValueRegex();
#endif

	readonly EnvOptions options;

	public Env(ILogger<IEnv> logger, IOptions<EnvOptions> options)
		: base(logger)
	{
		this.options = Guard.ArgumentNotNull(Guard.ArgumentNotNull(options).Value, nameof(options));

		var basePath = this.options.Path ?? AppContext.BaseDirectory;
		var local = this.options.Local ?? true;
		var environment = this.options.Environment ?? Environment.GetEnvironmentVariable(AppEnvironment);

		ParseEnvFile(Path.Combine(basePath, ".env"));
		if (local)
			ParseEnvFile(Path.Combine(basePath, ".env.local"));

		if (environment is not null)
		{
			ParseEnvFile(Path.Combine(basePath, $".env.{environment}"));
			if (local)
				ParseEnvFile(Path.Combine(basePath, $".env.{environment}.local"));
		}
	}

	/// <summary>
	/// A list of case-insensitive values that indicate a value is falsey.
	/// </summary>
	static internal readonly HashSet<string?> FalseValues = new(StringComparer.OrdinalIgnoreCase) { "false", "off", "no", "0" };

	/// <summary>
	/// A list of case-insensitive values that indicate a value is truthy.
	/// </summary>
	static internal readonly HashSet<string?> TrueValues = new(StringComparer.OrdinalIgnoreCase) { "true", "on", "yes", "1" };

	/// <summary>
	/// Gets a flag indicating whether environment variable <c>HYPHEN_DEV</c> is truthy.
	/// </summary>
	[ExcludeFromCodeCoverage]
	internal static bool IsDevEnvironment => TrueValues.Contains(Environment.GetEnvironmentVariable(Dev));

	public bool? GetBool(string name, [NotNullWhen(true)] bool required) =>
		TryParse<bool>("bool", name, required, value =>
		{
			if (FalseValues.Contains(value))
				return false;
			if (TrueValues.Contains(value))
				return true;
			return null;
		});

	public decimal? GetDecimal(string name, NumberStyles numberStyles, IFormatProvider? formatProvider, [NotNullWhen(true)] bool required) =>
		TryParse<decimal>("decimal", name, required, value =>
		{
			if (decimal.TryParse(value, numberStyles, formatProvider, out var parsed))
				return parsed;
			return null;
		});

	public double? GetDouble(string name, NumberStyles numberStyles, IFormatProvider? formatProvider, [NotNullWhen(true)] bool required) =>
		TryParse<double>("double", name, required, value =>
		{
			if (double.TryParse(value, numberStyles, formatProvider, out var parsed))
				return parsed;
			return null;
		});

	public int? GetInt(string name, NumberStyles numberStyles, IFormatProvider? formatProvider, [NotNullWhen(true)] bool required) =>
		TryParse<int>("int", name, required, value =>
		{
			if (int.TryParse(value, numberStyles, formatProvider, out var parsed))
				return parsed;
			return null;
		});

	public long? GetLong(string name, NumberStyles numberStyles, IFormatProvider? formatProvider, [NotNullWhen(true)] bool required) =>
		TryParse<long>("long", name, required, value =>
		{
			if (long.TryParse(value, numberStyles, formatProvider, out var parsed))
				return parsed;
			return null;
		});

	public string? GetString(string name, [NotNullWhen(true)] bool required)
	{
		var value = options.GetEnv(name);
		if (!required || value is not null)
			return value;

		throw new ArgumentException(HyphenSdkResources.Env_InvalidValue(name, value), nameof(name));
	}

	void ParseEnvFile(string path)
	{
		var lines = options.ReadFile(path);
		if (lines is null)
			return;

		var currentName = string.Empty;
		var currentValue = string.Empty;
		var currentTerminationRegex = default(Regex);

		for (var idx = 0; idx < lines.Length; ++idx)
		{
			var line = lines[idx];

			// Lines beginning with # are comments
			if (line.Length == 0 || line[0] == '#')
				continue;

			// If there's no current name, we start by getting the name
			if (currentName.Length == 0)
			{
				var nameMatch = nameRegex.Match(line);
				if (!nameMatch.Success)
				{
					Logger.LogWarning(HyphenSdkResources.Env_InvalidLineFmt, path, line);
					continue;
				}

				var name = nameMatch.Groups[1].Value;
				var valueMatch = valueRegex.Match(nameMatch.Groups[2].Value);
				Guard.True(valueMatch.Success, "This regex should always succeed");

				// Single line values
				var groupValues = valueMatch.Groups.Cast<Group>().Select(g => g.Value).ToArray();
				var (value, terminationRegex, escaped) = (groupValues[2], groupValues[5], groupValues[8], groupValues[11]) switch
				{
					({ Length: > 0 }, _, _, _) => (groupValues[2], groupValues[3].StartsWith('`') ? null : terminationBackQuoteRegex, false),
					(_, { Length: > 0 }, _, _) => (groupValues[5], groupValues[6].StartsWith('"') ? null : terminationDoubleQuoteRegex, true),
					(_, _, { Length: > 0 }, _) => (groupValues[8], groupValues[9].StartsWith('\'') ? null : terminationSingleQuoteRegex, false),
					_ => (groupValues[11].StartsWith('#') ? "" : groupValues[11], default, false),
				};

				if (escaped)
					value = value.ReplaceInvariant("\\r", "\r").ReplaceInvariant("\\n", "\n");

				if (terminationRegex is null)
					options.SetEnv(name, value.Length == 0 ? null : value);
				else
				{
					currentName = name;
					currentValue = value;
					currentTerminationRegex = terminationRegex;
				}
			}
			// This is the continuation of a multi-line value
			else
			{
				var terminationMatch = currentTerminationRegex!.Match(line);
				if (!terminationMatch.Success)
				{
					currentValue += Environment.NewLine + line;
					continue;
				}

				options.SetEnv(currentName, $"{currentValue}{Environment.NewLine}{terminationMatch.Groups[1].Value}");

				currentName = string.Empty;
				currentValue = string.Empty;
				currentTerminationRegex = default;
			}
		}

		if (currentName.Length != 0)
			Logger.LogWarning(HyphenSdkResources.Env_UnterminatedMultiLineValueFmt, path, currentName);
	}

	T? TryParse<T>(string typeName, string name, bool required, Func<string, T?> parser)
		where T : struct
	{
		var value = GetString(name, required);
		if (value is null)
			return null;

		var parsed = parser(value);
		if (parsed.HasValue)
			return parsed;
		if (required)
			throw new ArgumentException(HyphenSdkResources.Env_InvalidValueType(name, typeName, value), nameof(name));

		Logger.LogWarning(HyphenSdkResources.Env_InvalidValueTypeFmt, name, typeName, value.Quoted());
		return null;
	}

#if !NETSTANDARD
	[GeneratedRegex(NameRegexPattern)]
	private static partial Regex generateNameRegex();

	[GeneratedRegex(TerminationBackQuoteRegexPattern)]
	private static partial Regex generateTerminationBackQuoteRegex();

	[GeneratedRegex(TerminationDoubleQuoteRegexPattern)]
	private static partial Regex generateTerminationDoubleQuoteRegex();

	[GeneratedRegex(TerminationSingleQuoteRegexPattern)]
	private static partial Regex generateTerminationSingleQuoteRegex();

	[GeneratedRegex(ValueRegexPattern)]
	private static partial Regex generateValueRegex();
#endif
}
