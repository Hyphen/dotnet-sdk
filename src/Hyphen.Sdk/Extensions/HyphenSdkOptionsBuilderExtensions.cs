using Hyphen.Sdk.Resources;
using Microsoft.Extensions.Options;

namespace Hyphen.Sdk;

internal static class HyphenSdkOptionsBuilderExtensions
{
	public static OptionsBuilder<TOptions> ValidateApiKey<TOptions>(this OptionsBuilder<TOptions> builder)
		where TOptions : BaseServiceOptions =>
			builder
				.Validate(
					options =>
					{
						if (options.ApiKey is null)
						{
							if (Environment.GetEnvironmentVariable("HYPHEN_API_KEY") is string envApiKey)
								options.ApiKey = envApiKey;
							else
								return false;
						}

						return true;
					},
					HyphenSdkResources.API_KEY_REQUIRED
				)
				.Validate(
					options => !options.ApiKey.StartsWith("public_", StringComparison.InvariantCultureIgnoreCase),
					HyphenSdkResources.PUBLIC_API_KEY_SHOULD_NOT_BE_USED
				);
}
