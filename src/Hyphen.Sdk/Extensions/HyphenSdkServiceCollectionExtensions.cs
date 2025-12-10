using Hyphen.Sdk;
using Hyphen.Sdk.Internal;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for the Hyphen SDK.
/// </summary>
public static class HyphenSdkServiceCollectionExtensions
{
	/// <summary>
	/// Adds the <see cref="INetInfo"/> service to the <see cref="IServiceCollection"/>.
	/// </summary>
	/// <param name="services"/>
	/// <returns>The <see cref="IServiceCollection"/>.</returns>
	public static IServiceCollection AddNetInfo(this IServiceCollection services)
	{
		RegisterDependencies(Guard.ArgumentNotNull(services));

		services.TryAddSingleton<INetInfo, NetInfo>();
		services
			.AddOptionsWithValidateOnStart<NetInfoOptions>()
			.Configure(options =>
			{
				options.BaseUri ??= new("https://net.info");
			})
			.ValidateApiKey();

		return services;
	}

	static void RegisterDependencies(IServiceCollection services)
	{
		services.AddOptions();
		services.AddLogging();
		services.AddHttpClient();
	}
}
