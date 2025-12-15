using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net;

#if !NETSTANDARD
using System.Text;
#endif

namespace Hyphen.Sdk.Resources;

[ExcludeFromCodeCoverage]
internal static class HyphenSdkResourcesExtensions
{
#if !NETSTANDARD
	static readonly CompositeFormat httpStatusCodeErrorComposite = CompositeFormat.Parse(HyphenSdkResources.HttpStatusCodeErrorFmt);
#endif

	extension(HyphenSdkResources)
	{
		public static string HttpStatusCodeError(HttpStatusCode statusCode) =>
			string.Format(
				CultureInfo.CurrentCulture,
#if NETSTANDARD
				HyphenSdkResources.HttpStatusCodeErrorFmt,
#else
				httpStatusCodeErrorComposite,
#endif
				(int)statusCode,
				statusCode
			);
	}
}
