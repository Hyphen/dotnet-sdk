using System.Diagnostics.CodeAnalysis;
using Hyphen.Sdk;
using Hyphen.Sdk.Internal;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Hyphen SDK extensions for <see cref="IServiceCollection"/>.
/// </summary>
[ExcludeFromCodeCoverage]
public static class HyphenSdkServiceCollectionExtensions
{
	/// <summary>
	/// Adds the <see cref="IEnv"/> service to the <see cref="IServiceCollection"/>.
	/// </summary>
	/// <param name="services" />
	/// <returns>The <see cref="IServiceCollection"/>.</returns>
	public static IServiceCollection AddHyphenEnv(this IServiceCollection services)
	{
		Guard.ArgumentNotNull(services)
			.RegisterDependencies()
			.TryAddSingleton<IEnv, Env>();

		return services;
	}

	/// <summary>
	/// Adds the <see cref="INetInfo"/> service to the <see cref="IServiceCollection"/>.
	/// </summary>
	/// <param name="services"/>
	/// <returns>The <see cref="IServiceCollection"/>.</returns>
	public static IServiceCollection AddNetInfo(this IServiceCollection services)
	{
		Guard.ArgumentNotNull(services)
			.RegisterDependencies()
			.TryAddSingleton<INetInfo, NetInfo>();

		return services;
	}

	static IServiceCollection RegisterDependencies(this IServiceCollection services) =>
		services
			.AddOptions()
			.AddLogging()
			.AddHttpClient();
}
