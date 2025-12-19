using System.Globalization;
using System.Net;

#if !NETSTANDARD
using System.Text;
#endif

namespace Hyphen.Sdk.Resources;

internal static class HyphenSdkResourcesExtensions
{
#if !NETSTANDARD
	static readonly CompositeFormat http_StatusCodeErrorComposite = CompositeFormat.Parse(HyphenSdkResources.Http_StatusCodeErrorFmt);
	static readonly CompositeFormat env_InvalidValueComposite = CompositeFormat.Parse(HyphenSdkResources.Env_InvalidValueFmt);
	static readonly CompositeFormat env_InvalidValueTypeComposite = CompositeFormat.Parse(HyphenSdkResources.Env_InvalidValueTypeFmt);
#endif

	extension(HyphenSdkResources)
	{
		public static string Env_InvalidValue(string name, string? value) =>
			string.Format(
				CultureInfo.CurrentCulture,
#if NETSTANDARD
				HyphenSdkResources.Env_InvalidValueFmt,
#else
				env_InvalidValueComposite,
#endif
				name,
				value.Quoted()
			);

		public static string Env_InvalidValueType(string name, string typeName, string? value) =>
			string.Format(
				CultureInfo.CurrentCulture,
#if NETSTANDARD
				HyphenSdkResources.Env_InvalidValueTypeFmt,
#else
				env_InvalidValueTypeComposite,
#endif
				name,
				typeName,
				value.Quoted()
			);

		public static string Http_StatusCodeError(HttpStatusCode statusCode) =>
			string.Format(
				CultureInfo.CurrentCulture,
#if NETSTANDARD
				HyphenSdkResources.Http_StatusCodeErrorFmt,
#else
				http_StatusCodeErrorComposite,
#endif
				(int)statusCode,
				statusCode
			);
	}
}
