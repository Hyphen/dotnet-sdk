using System.Runtime.InteropServices;

namespace Hyphen.Sdk;

public class EnvTests
{
	readonly SpyLogger<IEnv> logger = new();

	public class Constructor : EnvTests
	{
		readonly string basePath;
		readonly EnvOptions options;
		readonly EnvHelper helper;

		public Constructor()
		{
			basePath =
				RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
					? @"C:\Dummy\Path"
					: @"/dummy/path";

			helper = new(basePath);
			options = new()
			{
				Path = basePath,
				ReadFile = helper.ReadFile,
				SetEnv = helper.SetEnv,
			};
		}

		[Fact]
		public void NoContent()
		{
			var _ = new Env(logger, Options.Create(options));

			Assert.Empty(logger.Messages);
			Assert.Empty(helper.SetEnvValues);
		}

		[Fact]
		public void UsesAppContextBaseDirectoryByDefault_WithLocal()
		{
			var helper = new EnvHelper();
			var options = new EnvOptions() { ReadFile = helper.ReadFile, SetEnv = helper.SetEnv, Local = true };

			var _ = new Env(logger, Options.Create(options));

			Assert.Collection(
				helper.ReadFileRequests,
				path => Assert.Equal(Path.Combine(AppContext.BaseDirectory, ".env"), path),
				path => Assert.Equal(Path.Combine(AppContext.BaseDirectory, ".env.local"), path)
			);
		}

		[Fact]
		public void UsesAppContextBaseDirectoryByDefault_WithoutLocal()
		{
			var helper = new EnvHelper();
			var options = new EnvOptions() { ReadFile = helper.ReadFile, SetEnv = helper.SetEnv, Local = false };

			var _ = new Env(logger, Options.Create(options));

			var path = Assert.Single(helper.ReadFileRequests);
			Assert.Equal(Path.Combine(AppContext.BaseDirectory, ".env"), path);
		}

		[Fact]
		public void SingleFile()
		{
			helper.AddFile(".env", """
				EMPTY=
				EMPTY_WITH_COMMENT= # comment

				# Quoting
				UNQUOTED=Value
				UNQUOTED_WITH_COMMENT=Value# comment
				UNQUOTED_WITH_SPACES =    Value 1 # comment
				UNQUOTED_JSON={ "foo": "2112" }

				SINGLE_QUOTED='Value'
				SINGLE_QUOTED_ALT='Value# not a comment'
				SINGLE_QUOTED_WITH_COMMENT='Value'# comment
				SINGLE_QUOTED_WITH_SPACES = '   Value 1 ' # comment
				SINGLE_QUOTED_JSON='{ "foo": "2112" }'

				DOUBLE_QUOTED="Value"
				DOUBLE_QUOTED_ALT="Value# not a comment"
				DOUBLE_QUOTED_WITH_COMMENT="Value"# comment
				DOUBLE_QUOTED_WITH_SPACES = "  Value 1  " # comment
				DOUBLE_QUOTED_WITH_ESCAPES = "This is\na multiline\nvalue"

				BACK_QUOTED=`Value`
				BACK_QUOTED_ALT=`Value# not a comment`
				BACK_QUOTED_WITH_COMMENT=`Value`# comment
				BACK_QUOTED_WITH_SPACES = ` Value 1   ` # comment
				BACK_QUOTED_JSON=`{ "foo": "2112" }`

				# Multiline
				MULTILINE_SINGLE_QUOTED='This is
				a single quoted
				value'
				MULTILINE_SINGLE_QUOTED_WITH_COMMENT='This is
				a single quoted
				value'# comment
				MULTILINE_DOUBLE_QUOTED="This is
				a double quoted
				value"
				MULTILINE_DOUBLE_QUOTED_WITH_COMMENT="This is
				a double quoted
				value"#comment
				MULTILINE_BACK_QUOTED=`This is
				a back quoted
				value`
				MULTILINE_BACK_QUOTED_WITH_COMMENT=`This is
				a back quoted
				value`# comment
				""");

			var _ = new Env(logger, Options.Create(options));

			Assert.Empty(logger.Messages);
			Assert.Collection(
				helper.SetEnvValues.OrderBy(kvp => kvp.Key).Select(kvp => $"{kvp.Key} = {kvp.Value.Quoted}"),
				item => Assert.Equal(@"BACK_QUOTED = ""Value""", item),
				item => Assert.Equal(@"BACK_QUOTED_ALT = ""Value# not a comment""", item),
				item => Assert.Equal(@"BACK_QUOTED_JSON = ""{ ""foo"": ""2112"" }""", item),
				item => Assert.Equal(@"BACK_QUOTED_WITH_COMMENT = ""Value""", item),
				item => Assert.Equal(@"BACK_QUOTED_WITH_SPACES = "" Value 1   """, item),

				item => Assert.Equal(@"DOUBLE_QUOTED = ""Value""", item),
				item => Assert.Equal(@"DOUBLE_QUOTED_ALT = ""Value# not a comment""", item),
				item => Assert.Equal(@"DOUBLE_QUOTED_WITH_COMMENT = ""Value""", item),
				item => Assert.Equal($@"DOUBLE_QUOTED_WITH_ESCAPES = ""This is{'\n'}a multiline{'\n'}value""", item),
				item => Assert.Equal(@"DOUBLE_QUOTED_WITH_SPACES = ""  Value 1  """, item),

				item => Assert.Equal(@"EMPTY = null", item),
				item => Assert.Equal(@"EMPTY_WITH_COMMENT = null", item),

				item => Assert.Equal($@"MULTILINE_BACK_QUOTED = ""This is{Environment.NewLine}a back quoted{Environment.NewLine}value""", item),
				item => Assert.Equal($@"MULTILINE_BACK_QUOTED_WITH_COMMENT = ""This is{Environment.NewLine}a back quoted{Environment.NewLine}value""", item),

				item => Assert.Equal($@"MULTILINE_DOUBLE_QUOTED = ""This is{Environment.NewLine}a double quoted{Environment.NewLine}value""", item),
				item => Assert.Equal($@"MULTILINE_DOUBLE_QUOTED_WITH_COMMENT = ""This is{Environment.NewLine}a double quoted{Environment.NewLine}value""", item),

				item => Assert.Equal($@"MULTILINE_SINGLE_QUOTED = ""This is{Environment.NewLine}a single quoted{Environment.NewLine}value""", item),
				item => Assert.Equal($@"MULTILINE_SINGLE_QUOTED_WITH_COMMENT = ""This is{Environment.NewLine}a single quoted{Environment.NewLine}value""", item),

				item => Assert.Equal(@"SINGLE_QUOTED = ""Value""", item),
				item => Assert.Equal(@"SINGLE_QUOTED_ALT = ""Value# not a comment""", item),
				item => Assert.Equal(@"SINGLE_QUOTED_JSON = ""{ ""foo"": ""2112"" }""", item),
				item => Assert.Equal(@"SINGLE_QUOTED_WITH_COMMENT = ""Value""", item),
				item => Assert.Equal(@"SINGLE_QUOTED_WITH_SPACES = ""   Value 1 """, item),

				item => Assert.Equal(@"UNQUOTED = ""Value""", item),
				item => Assert.Equal(@"UNQUOTED_JSON = ""{ ""foo"": ""2112"" }""", item),
				item => Assert.Equal(@"UNQUOTED_WITH_COMMENT = ""Value""", item),
				item => Assert.Equal(@"UNQUOTED_WITH_SPACES = ""Value 1""", item)
			);
		}

		[Fact]
		public void LocalOverrides()
		{
			helper.AddFile(".env", """
				OVERRIDE=foo
				GLOBAL=bar
				""");
			helper.AddFile(".env.local", """
				OVERRIDE=baz
				LOCAL=biff
				""");

			var _ = new Env(logger, Options.Create(options));

			Assert.Empty(logger.Messages);
			Assert.Collection(
				helper.SetEnvValues.OrderBy(kvp => kvp.Key).Select(kvp => $"{kvp.Key} = {kvp.Value.Quoted}"),
				item => Assert.Equal(@"GLOBAL = ""bar""", item),
				item => Assert.Equal(@"LOCAL = ""biff""", item),
				item => Assert.Equal(@"OVERRIDE = ""baz""", item)
			);
		}

		[Fact]
		public void EnvironmentOverrides()
		{
			options.Environment = "production";
			helper.AddFile(".env", """
				NAME1=global
				NAME2=global
				NAME3=global
				NAME4=global
				""");
			helper.AddFile(".env.local", """
				NAME2=local
				NAME3=local
				NAME4=local
				""");
			helper.AddFile(".env.production", """
				NAME3=environment
				NAME4=environment
				""");
			helper.AddFile(".env.production.local", """
				NAME4=environment-local
				""");

			var _ = new Env(logger, Options.Create(options));

			Assert.Empty(logger.Messages);
			Assert.Collection(
				helper.SetEnvValues.OrderBy(kvp => kvp.Key).Select(kvp => $"{kvp.Key} = {kvp.Value.Quoted}"),
				item => Assert.Equal(@"NAME1 = ""global""", item),
				item => Assert.Equal(@"NAME2 = ""local""", item),
				item => Assert.Equal(@"NAME3 = ""environment""", item),
				item => Assert.Equal(@"NAME4 = ""environment-local""", item)
			);
		}

		[Fact]
		public void BadValues()
		{
			helper.AddFile(".env", """
				BAD-NAME=foo
				NAME_WITHOUT_VALUE
				=Value without name
				NAME=`Unterminated
				multiline
				value
				""");

			var _ = new Env(logger, Options.Create(options));

			var dotEnvPath = Path.Combine(basePath, ".env");
			Assert.Collection(
				logger.Messages,
				message => Assert.Equal($@"[Warning] While parsing ""{dotEnvPath}"", line ""BAD-NAME=foo"" was not valid.", message),
				message => Assert.Equal($@"[Warning] While parsing ""{dotEnvPath}"", line ""NAME_WITHOUT_VALUE"" was not valid.", message),
				message => Assert.Equal($@"[Warning] While parsing ""{dotEnvPath}"", line ""=Value without name"" was not valid.", message),
				message => Assert.Equal($@"[Warning] While parsing ""{dotEnvPath}"", value ""NAME"" has an unterminated multi-line value.", message)
			);
		}
	}

	public class GetBool : EnvTests
	{
		readonly EnvHelper helper = new();
		readonly EnvOptions options;

		public GetBool() =>
			options = new() { GetEnv = helper.GetEnv };

		[Fact]
		public void UnsetValue_Optional()
		{
			var env = new Env(logger, Options.Create(options));

			var result = env.GetBool("UNSET", required: false);

			Assert.Empty(logger.Messages);
			Assert.Null(result);
		}

		[Fact]
		public void UnsetValue_Required()
		{
			var env = new Env(logger, Options.Create(options));

			var ex = Record.Exception(() => env.GetBool("UNSET", required: true));

			Assert.Empty(logger.Messages);
			var argEx = Assert.IsType<ArgumentException>(ex);
			Assert.StartsWith(@"Environment variable ""UNSET"" has an invalid value (null).", argEx.Message);
			Assert.Equal("name", argEx.ParamName);
		}

		[Theory]
		[InlineData("true", true)]
		[InlineData("on", true)]
		[InlineData("yes", true)]
		[InlineData("1", true)]
		[InlineData("false", false)]
		[InlineData("off", false)]
		[InlineData("no", false)]
		[InlineData("0", false)]
		public void SetValue_Valid(string value, bool expected)
		{
			helper.SetEnv("SET", value);
			var env = new Env(logger, Options.Create(options));

			var result = env.GetBool("SET");

			Assert.Empty(logger.Messages);
			Assert.Equal(expected, result);
		}

		[Fact]
		public void SetValue_Invalid_Optional()
		{
			helper.SetEnv("SET", "foo");
			var env = new Env(logger, Options.Create(options));

			var result = env.GetBool("SET", required: false);

			Assert.Equal(@"[Warning] Environment variable ""SET"" has an invalid bool value (""foo"").", Assert.Single(logger.Messages));
			Assert.Null(result);
		}

		[Fact]
		public void SetValue_Invalid_Required()
		{
			helper.SetEnv("SET", "foo");
			var env = new Env(logger, Options.Create(options));

			var ex = Record.Exception(() => env.GetBool("SET", required: true));

			Assert.Empty(logger.Messages);
			var argEx = Assert.IsType<ArgumentException>(ex);
			Assert.StartsWith(@"Environment variable ""SET"" has an invalid bool value (""foo"").", argEx.Message);
			Assert.Equal("name", argEx.ParamName);
		}
	}

	public class GetDecimal : EnvTests
	{
		readonly EnvHelper helper = new();
		readonly EnvOptions options;

		public GetDecimal() =>
			options = new() { GetEnv = helper.GetEnv };

		[Fact]
		public void UnsetValue_Optional()
		{
			var env = new Env(logger, Options.Create(options));

			var result = env.GetDecimal("UNSET", required: false);

			Assert.Empty(logger.Messages);
			Assert.Null(result);
		}

		[Fact]
		public void UnsetValue_Required()
		{
			var env = new Env(logger, Options.Create(options));

			var ex = Record.Exception(() => env.GetDecimal("UNSET", required: true));

			Assert.Empty(logger.Messages);
			var argEx = Assert.IsType<ArgumentException>(ex);
			Assert.StartsWith(@"Environment variable ""UNSET"" has an invalid value (null).", argEx.Message);
			Assert.Equal("name", argEx.ParamName);
		}

		[Theory]
		[InlineData("0")]
		[InlineData("0.1")]
		[InlineData("-21.12")]
		public void SetValue_Valid(string value)
		{
			helper.SetEnv("SET", value);
			var env = new Env(logger, Options.Create(options));

			var result = env.GetDecimal("SET");

			Assert.Empty(logger.Messages);
			Assert.Equal(decimal.Parse(value), result);
		}

		[Fact]
		public void SetValue_Invalid_Optional()
		{
			helper.SetEnv("SET", "foo");
			var env = new Env(logger, Options.Create(options));

			var result = env.GetDecimal("SET", required: false);

			Assert.Equal(@"[Warning] Environment variable ""SET"" has an invalid decimal value (""foo"").", Assert.Single(logger.Messages));
			Assert.Null(result);
		}

		[Fact]
		public void SetValue_Invalid_Required()
		{
			helper.SetEnv("SET", "foo");
			var env = new Env(logger, Options.Create(options));

			var ex = Record.Exception(() => env.GetDecimal("SET", required: true));

			Assert.Empty(logger.Messages);
			var argEx = Assert.IsType<ArgumentException>(ex);
			Assert.StartsWith(@"Environment variable ""SET"" has an invalid decimal value (""foo"").", argEx.Message);
			Assert.Equal("name", argEx.ParamName);
		}
	}

	public class GetDouble : EnvTests
	{
		readonly EnvHelper helper = new();
		readonly EnvOptions options;

		public GetDouble() =>
			options = new() { GetEnv = helper.GetEnv };

		[Fact]
		public void UnsetValue_Optional()
		{
			var env = new Env(logger, Options.Create(options));

			var result = env.GetDouble("UNSET", required: false);

			Assert.Empty(logger.Messages);
			Assert.Null(result);
		}

		[Fact]
		public void UnsetValue_Required()
		{
			var env = new Env(logger, Options.Create(options));

			var ex = Record.Exception(() => env.GetDouble("UNSET", required: true));

			Assert.Empty(logger.Messages);
			var argEx = Assert.IsType<ArgumentException>(ex);
			Assert.StartsWith(@"Environment variable ""UNSET"" has an invalid value (null).", argEx.Message);
			Assert.Equal("name", argEx.ParamName);
		}

		[Theory]
		[InlineData("0")]
		[InlineData("0.1")]
		[InlineData("-21.12")]
		public void SetValue_Valid(string value)
		{
			helper.SetEnv("SET", value);
			var env = new Env(logger, Options.Create(options));

			var result = env.GetDouble("SET");

			Assert.Empty(logger.Messages);
			Assert.Equal(double.Parse(value), result);
		}

		[Fact]
		public void SetValue_Invalid_Optional()
		{
			helper.SetEnv("SET", "foo");
			var env = new Env(logger, Options.Create(options));

			var result = env.GetDouble("SET", required: false);

			Assert.Equal(@"[Warning] Environment variable ""SET"" has an invalid double value (""foo"").", Assert.Single(logger.Messages));
			Assert.Null(result);
		}

		[Fact]
		public void SetValue_Invalid_Required()
		{
			helper.SetEnv("SET", "foo");
			var env = new Env(logger, Options.Create(options));

			var ex = Record.Exception(() => env.GetDouble("SET", required: true));

			Assert.Empty(logger.Messages);
			var argEx = Assert.IsType<ArgumentException>(ex);
			Assert.StartsWith(@"Environment variable ""SET"" has an invalid double value (""foo"").", argEx.Message);
			Assert.Equal("name", argEx.ParamName);
		}
	}

	public class GetInt : EnvTests
	{
		readonly EnvHelper helper = new();
		readonly EnvOptions options;

		public GetInt() =>
			options = new() { GetEnv = helper.GetEnv };

		[Fact]
		public void UnsetValue_Optional()
		{
			var env = new Env(logger, Options.Create(options));

			var result = env.GetInt("UNSET", required: false);

			Assert.Empty(logger.Messages);
			Assert.Null(result);
		}

		[Fact]
		public void UnsetValue_Required()
		{
			var env = new Env(logger, Options.Create(options));

			var ex = Record.Exception(() => env.GetInt("UNSET", required: true));

			Assert.Empty(logger.Messages);
			var argEx = Assert.IsType<ArgumentException>(ex);
			Assert.StartsWith(@"Environment variable ""UNSET"" has an invalid value (null).", argEx.Message);
			Assert.Equal("name", argEx.ParamName);
		}

		[Theory]
		[InlineData("0")]
		[InlineData("42")]
		[InlineData("-2112")]
		public void SetValue_Valid(string value)
		{
			helper.SetEnv("SET", value);
			var env = new Env(logger, Options.Create(options));

			var result = env.GetInt("SET");

			Assert.Empty(logger.Messages);
			Assert.Equal(int.Parse(value), result);
		}

		[Fact]
		public void SetValue_Invalid_Optional()
		{
			helper.SetEnv("SET", "foo");
			var env = new Env(logger, Options.Create(options));

			var result = env.GetInt("SET", required: false);

			Assert.Equal(@"[Warning] Environment variable ""SET"" has an invalid int value (""foo"").", Assert.Single(logger.Messages));
			Assert.Null(result);
		}

		[Fact]
		public void SetValue_Invalid_Required()
		{
			helper.SetEnv("SET", "foo");
			var env = new Env(logger, Options.Create(options));

			var ex = Record.Exception(() => env.GetInt("SET", required: true));

			Assert.Empty(logger.Messages);
			var argEx = Assert.IsType<ArgumentException>(ex);
			Assert.StartsWith(@"Environment variable ""SET"" has an invalid int value (""foo"").", argEx.Message);
			Assert.Equal("name", argEx.ParamName);
		}
	}

	public class GetLong : EnvTests
	{
		readonly EnvHelper helper = new();
		readonly EnvOptions options;

		public GetLong() =>
			options = new() { GetEnv = helper.GetEnv };

		[Fact]
		public void UnsetValue_Optional()
		{
			var env = new Env(logger, Options.Create(options));

			var result = env.GetLong("UNSET", required: false);

			Assert.Empty(logger.Messages);
			Assert.Null(result);
		}

		[Fact]
		public void UnsetValue_Required()
		{
			var env = new Env(logger, Options.Create(options));

			var ex = Record.Exception(() => env.GetLong("UNSET", required: true));

			Assert.Empty(logger.Messages);
			var argEx = Assert.IsType<ArgumentException>(ex);
			Assert.StartsWith(@"Environment variable ""UNSET"" has an invalid value (null).", argEx.Message);
			Assert.Equal("name", argEx.ParamName);
		}

		[Theory]
		[InlineData("0")]
		[InlineData("123456789012")]
		[InlineData("-123456789012")]
		public void SetValue_Valid(string value)
		{
			helper.SetEnv("SET", value);
			var env = new Env(logger, Options.Create(options));

			var result = env.GetLong("SET");

			Assert.Empty(logger.Messages);
			Assert.Equal(long.Parse(value), result);
		}

		[Fact]
		public void SetValue_Invalid_Optional()
		{
			helper.SetEnv("SET", "foo");
			var env = new Env(logger, Options.Create(options));

			var result = env.GetLong("SET", required: false);

			Assert.Equal(@"[Warning] Environment variable ""SET"" has an invalid long value (""foo"").", Assert.Single(logger.Messages));
			Assert.Null(result);
		}

		[Fact]
		public void SetValue_Invalid_Required()
		{
			helper.SetEnv("SET", "foo");
			var env = new Env(logger, Options.Create(options));

			var ex = Record.Exception(() => env.GetLong("SET", required: true));

			Assert.Empty(logger.Messages);
			var argEx = Assert.IsType<ArgumentException>(ex);
			Assert.StartsWith(@"Environment variable ""SET"" has an invalid long value (""foo"").", argEx.Message);
			Assert.Equal("name", argEx.ParamName);
		}
	}

	public class GetString : EnvTests
	{
		readonly EnvHelper helper = new();
		readonly EnvOptions options;

		public GetString() =>
			options = new() { GetEnv = helper.GetEnv };

		[Fact]
		public void UnsetValue_Optional()
		{
			var env = new Env(logger, Options.Create(options));

			var result = env.GetString("UNSET", required: false);

			Assert.Empty(logger.Messages);
			Assert.Null(result);
		}

		[Fact]
		public void UnsetValue_Required()
		{
			var env = new Env(logger, Options.Create(options));

			var ex = Record.Exception(() => env.GetString("UNSET", required: true));

			Assert.Empty(logger.Messages);
			var argEx = Assert.IsType<ArgumentException>(ex);
			Assert.StartsWith(@"Environment variable ""UNSET"" has an invalid value (null).", argEx.Message);
			Assert.Equal("name", argEx.ParamName);
		}

		[Theory]
		[InlineData("0")]
		[InlineData("123456789012")]
		[InlineData("-123456789012")]
		public void SetValue(string value)
		{
			helper.SetEnv("SET", value);
			var env = new Env(logger, Options.Create(options));

			var result = env.GetString("SET");

			Assert.Empty(logger.Messages);
			Assert.Equal(value, result);
		}
	}

	class EnvHelper(string? basePath = null)
	{
		readonly Dictionary<string, string[]> linesByPath = [];

		public readonly List<string> ReadFileRequests = [];
		public readonly Dictionary<string, string?> SetEnvValues = [];

		public void AddFile(string path, string content) =>
			linesByPath[GetPath(path)] = content.Split('\r', '\n');

		public string? GetEnv(string key)
		{
			SetEnvValues.TryGetValue(key, out var result);
			return result;
		}

		string GetPath(string path) =>
			Path.Combine(basePath ?? AppContext.BaseDirectory, path);

		public string[]? ReadFile(string path)
		{
			ReadFileRequests.Add(path);

			if (linesByPath.TryGetValue(GetPath(path), out var lines))
				return lines;
			return null;
		}

		public void SetEnv(string key, string? value) =>
			SetEnvValues[key] = value;
	}
}
