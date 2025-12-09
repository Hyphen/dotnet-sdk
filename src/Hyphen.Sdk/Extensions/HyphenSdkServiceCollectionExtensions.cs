using System.Diagnostics.CodeAnalysis;
using Hyphen.Sdk;
using Hyphen.Sdk.Resources;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for the Hyphen SDK.
/// </summary>
[SuppressMessage("Design", "CA1034:Nested types should not be visible", Justification = "Extension blocks are implemented as nested types")]
public static class HyphenSdkServiceCollectionExtensions
{
	extension(IServiceCollection services)
	{
		/// <summary>
		/// Adds the <see cref="INetInfo"/> service to the <see cref="IServiceCollection"/>.
		/// </summary>
		/// <returns>The <see cref="IServiceCollection"/>.</returns>
		public IServiceCollection AddNetInfo() =>
			AddNetInfo(services, default!);

		/// <summary>
		/// Adds the <see cref="INetInfo"/> service to the <see cref="IServiceCollection"/>, allowing
		/// the user to configure the service.
		/// </summary>
		/// <param name="configure">The action used to configure the options.</param>
		/// <returns>The <see cref="IServiceCollection"/>.</returns>
		public IServiceCollection AddNetInfo(Action<NetInfoOptions> configure)
		{
			RegisterDependencies(services);

			services.TryAddSingleton<INetInfo, NetInfo>();
			services
				.AddOptionsWithValidateOnStart<NetInfoOptions>()
				.Configure(options =>
				{
					configure?.Invoke(options);

					options.BaseUri ??= new("https://net.info");
				})
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

			return services;
		}
	}

	static void RegisterDependencies(IServiceCollection services)
	{
		services.AddOptions();
		services.AddLogging();
		services.AddHttpClient();
	}
}
