using System.Diagnostics.CodeAnalysis;

namespace Hyphen.Sdk;

/// <summary>
/// Hyphen SDK extensions for <see cref="ILink"/>.
/// </summary>
[ExcludeFromCodeCoverage]
public static class HyphenSdkLinkExtensions
{
	/// <summary>
	/// Creates a QR code which points at a short code link.
	/// </summary>
	/// <param name="link"/>
	/// <param name="shortCodeId">The short code ID. (Example: <c>"code_686bed403c3991bd676bba4d"</c>)</param>
	/// <returns>Returns the QR code that was created.</returns>
	/// <exception cref="ApiKeyException">Thrown if the API key is not valid.</exception>
	/// <exception cref="NotFoundException">Thrown for an unknown short code ID or organization ID.</exception>
	/// <exception cref="HttpStatusCodeException">Thrown if the API returned an unexpected status code.</exception>
	public static Task<QrCodeResult> CreateQrCode(this ILink link, string shortCodeId) =>
		Guard.ArgumentNotNull(link).CreateQrCode(shortCodeId, default, default);

	/// <summary>
	/// Creates a QR code which points at a short code link.
	/// </summary>
	/// <param name="link"/>
	/// <param name="shortCodeId">The short code ID. (Example: <c>"code_686bed403c3991bd676bba4d"</c>)</param>
	/// <param name="cancellationToken">The cancellation token to cancel the request early</param>
	/// <returns>Returns the QR code that was created.</returns>
	/// <exception cref="ApiKeyException">Thrown if the API key is not valid.</exception>
	/// <exception cref="NotFoundException">Thrown for an unknown short code ID or organization ID.</exception>
	/// <exception cref="HttpStatusCodeException">Thrown if the API returned an unexpected status code.</exception>
	public static Task<QrCodeResult> CreateQrCode(this ILink link, string shortCodeId, CancellationToken cancellationToken) =>
		Guard.ArgumentNotNull(link).CreateQrCode(shortCodeId, default, cancellationToken);

	/// <summary>
	/// Creates a QR code which points at a short code link.
	/// </summary>
	/// <param name="link"/>
	/// <param name="shortCodeId">The short code ID. (Example: <c>"code_686bed403c3991bd676bba4d"</c>)</param>
	/// <param name="parms">Optional parameters for creating the QR code.</param>
	/// <returns>Returns the QR code that was created.</returns>
	/// <exception cref="ApiKeyException">Thrown if the API key is not valid.</exception>
	/// <exception cref="NotFoundException">Thrown for an unknown short code ID or organization ID.</exception>
	/// <exception cref="HttpStatusCodeException">Thrown if the API returned an unexpected status code.</exception>
	public static Task<QrCodeResult> CreateQrCode(this ILink link, string shortCodeId, CreateQrCodeParams? parms) =>
		Guard.ArgumentNotNull(link).CreateQrCode(shortCodeId, parms, default);

	/// <summary>
	/// Creates a short code for a long URL.
	/// </summary>
	/// <param name="link"/>
	/// <param name="longUrl">The long URL to shorten.</param>
	/// <param name="domain">The domain to use for the short code.</param>
	/// <returns>Returns the short code that was created.</returns>
	/// <exception cref="ApiKeyException">Thrown if the API key is not valid.</exception>
	/// <exception cref="NotFoundException">Thrown for an unknown organization ID.</exception>
	/// <exception cref="HttpStatusCodeException">Thrown if the API returned an unexpected status code.</exception>
	public static Task<ShortCodeResult> CreateShortCode(this ILink link, Uri longUrl, string domain) =>
		Guard.ArgumentNotNull(link).CreateShortCode(longUrl, domain, default, default);

	/// <summary>
	/// Creates a short code for a long URL.
	/// </summary>
	/// <param name="link"/>
	/// <param name="longUrl">The long URL to shorten.</param>
	/// <param name="domain">The domain to use for the short code.</param>
	/// <param name="cancellationToken">The cancellation token to cancel the request early</param>
	/// <returns>Returns the short code that was created.</returns>
	/// <exception cref="ApiKeyException">Thrown if the API key is not valid.</exception>
	/// <exception cref="NotFoundException">Thrown for an unknown organization ID.</exception>
	/// <exception cref="HttpStatusCodeException">Thrown if the API returned an unexpected status code.</exception>
	public static Task<ShortCodeResult> CreateShortCode(this ILink link, Uri longUrl, string domain, CancellationToken cancellationToken) =>
		Guard.ArgumentNotNull(link).CreateShortCode(longUrl, domain, default, cancellationToken);

	/// <summary>
	/// Creates a short code for a long URL.
	/// </summary>
	/// <param name="link"/>
	/// <param name="longUrl">The long URL to shorten.</param>
	/// <param name="domain">The domain to use for the short code.</param>
	/// <param name="parms">Optional parameters for creating the short code.</param>
	/// <returns>Returns the short code that was created.</returns>
	/// <exception cref="ApiKeyException">Thrown if the API key is not valid.</exception>
	/// <exception cref="NotFoundException">Thrown for an unknown organization ID.</exception>
	/// <exception cref="HttpStatusCodeException">Thrown if the API returned an unexpected status code.</exception>
	public static Task<ShortCodeResult> CreateShortCode(this ILink link, Uri longUrl, string domain, CreateShortCodeParams? parms) =>
		Guard.ArgumentNotNull(link).CreateShortCode(longUrl, domain, parms, default);

	/// <summary>
	/// Deletes a QR code.
	/// </summary>
	/// <param name="link"/>
	/// <param name="shortCodeId">The short code ID. (Example: <c>"code_686bed403c3991bd676bba4d"</c>)</param>
	/// <param name="qrCodeId">The QR code ID. (Example: <c>"lqr_66fc51fe144cf3a1bd2a35b1"</c>)</param>
	/// <exception cref="ApiKeyException">Thrown if the API key is not valid.</exception>
	/// <exception cref="HttpStatusCodeException">Thrown if the API returned an unexpected status code.</exception>
	public static Task DeleteQrCode(this ILink link, string shortCodeId, string qrCodeId) =>
		Guard.ArgumentNotNull(link).DeleteQrCode(shortCodeId, qrCodeId, default);

	/// <summary>
	/// Deletes a short code.
	/// </summary>
	/// <param name="link"/>
	/// <param name="shortCodeId">The short code ID. (Example: <c>"code_686bed403c3991bd676bba4d"</c>)</param>
	/// <exception cref="ApiKeyException">Thrown if the API key is not valid.</exception>
	/// <exception cref="HttpStatusCodeException">Thrown if the API returned an unexpected status code.</exception>
	public static Task DeleteShortCode(this ILink link, string shortCodeId) =>
		Guard.ArgumentNotNull(link).DeleteShortCode(shortCodeId, default);

	/// <summary>
	/// Gets details about a short code.
	/// </summary>
	/// <param name="link"/>
	/// <param name="shortCodeId">The short code ID. (Example: <c>"code_686bed403c3991bd676bba4d"</c>)</param>
	/// <param name="qrCodeId">The QR code ID. (Example: <c>"lqr_66fc51fe144cf3a1bd2a35b1"</c>)</param>
	/// <returns>Returns the QR code, if present, returns <c>default</c> for an unknown short code ID
	/// or QR code ID or organization ID.</returns>
	/// <exception cref="ApiKeyException">Thrown if the API key is not valid.</exception>
	/// <exception cref="HttpStatusCodeException">Thrown if the API returned an unexpected status code.</exception>
	public static Task<QrCodeResult?> GetQrCode(this ILink link, string shortCodeId, string qrCodeId) =>
		Guard.ArgumentNotNull(link).GetQrCode(shortCodeId, qrCodeId, default);

	/// <summary>
	/// Gets a list of QR codes associated with a short code.
	/// </summary>
	/// <param name="link"/>
	/// <param name="shortCodeId">The short code ID. (Example: <c>"code_686bed403c3991bd676bba4d"</c>)</param>
	/// <returns>The list of QR codes that match the search options; returns <c>null</c> for an unknown
	/// short code ID or organization ID.</returns>
	/// <exception cref="ApiKeyException">Thrown if the API key is not valid.</exception>
	/// <exception cref="HttpStatusCodeException">Thrown if the API returned an unexpected status code.</exception>
	public static Task<PagedResult<QrCodeResult>?> GetQrCodes(this ILink link, string shortCodeId) =>
		Guard.ArgumentNotNull(link).GetQrCodes(shortCodeId, default, default);

	/// <summary>
	/// Gets a list of QR codes associated with a short code.
	/// </summary>
	/// <param name="link"/>
	/// <param name="shortCodeId">The short code ID. (Example: <c>"code_686bed403c3991bd676bba4d"</c>)</param>
	/// <param name="cancellationToken">The cancellation token to cancel the request early</param>
	/// <returns>The list of QR codes that match the search options; returns <c>null</c> for an unknown
	/// short code ID or organization ID.</returns>
	/// <exception cref="ApiKeyException">Thrown if the API key is not valid.</exception>
	/// <exception cref="HttpStatusCodeException">Thrown if the API returned an unexpected status code.</exception>
	public static Task<PagedResult<QrCodeResult>?> GetQrCodes(this ILink link, string shortCodeId, CancellationToken cancellationToken) =>
		Guard.ArgumentNotNull(link).GetQrCodes(shortCodeId, default, cancellationToken);

	/// <summary>
	/// Gets a list of QR codes associated with a short code.
	/// </summary>
	/// <param name="link"/>
	/// <param name="shortCodeId">The short code ID. (Example: <c>"code_686bed403c3991bd676bba4d"</c>)</param>
	/// <param name="parms">Optional parameters for getting the list of QR codes.</param>
	/// <returns>The list of QR codes that match the search options; returns <c>null</c> for an unknown
	/// short code ID or organization ID.</returns>
	/// <exception cref="ApiKeyException">Thrown if the API key is not valid.</exception>
	/// <exception cref="HttpStatusCodeException">Thrown if the API returned an unexpected status code.</exception>
	public static Task<PagedResult<QrCodeResult>?> GetQrCodes(this ILink link, string shortCodeId, GetQrCodesParams? parms) =>
		Guard.ArgumentNotNull(link).GetQrCodes(shortCodeId, parms, default);

	/// <summary>
	/// Gets a short code.
	/// </summary>
	/// <param name="link"/>
	/// <param name="shortCodeId">The short code ID. (Example: <c>"code_686bed403c3991bd676bba4d"</c>)</param>
	/// <returns>Returns the short code, if available; returns <c>null</c> for an unknown code or organization ID.</returns>
	/// <exception cref="ApiKeyException">Thrown if the API key is not valid.</exception>
	/// <exception cref="HttpStatusCodeException">Thrown if the API returned an unexpected status code.</exception>
	public static Task<ShortCodeResult?> GetShortCode(this ILink link, string shortCodeId) =>
		Guard.ArgumentNotNull(link).GetShortCode(shortCodeId, default);

	/// <summary>
	/// Gets a list of short codes.
	/// </summary>
	/// <param name="link"/>
	/// <returns>The list of short codes that match the search options; returns <c>null</c> for an unknown organization ID.</returns>
	/// <exception cref="ApiKeyException">Thrown if the API key is not valid.</exception>
	/// <exception cref="HttpStatusCodeException">Thrown if the API returned an unexpected status code.</exception>
	public static Task<PagedResult<ShortCodeResult>?> GetShortCodes(this ILink link) =>
		Guard.ArgumentNotNull(link).GetShortCodes(default, default);

	/// <summary>
	/// Gets a list of short codes.
	/// </summary>
	/// <param name="link"/>
	/// <param name="cancellationToken">The cancellation token to cancel the request early</param>
	/// <returns>The list of short codes that match the search options; returns <c>null</c> for an unknown organization ID.</returns>
	/// <exception cref="ApiKeyException">Thrown if the API key is not valid.</exception>
	/// <exception cref="HttpStatusCodeException">Thrown if the API returned an unexpected status code.</exception>
	public static Task<PagedResult<ShortCodeResult>?> GetShortCodes(this ILink link, CancellationToken cancellationToken) =>
		Guard.ArgumentNotNull(link).GetShortCodes(default, cancellationToken);

	/// <summary>
	/// Gets a list of short codes.
	/// </summary>
	/// <param name="link"/>
	/// <param name="parms">Optional parameters for getting the list of short codes.</param>
	/// <returns>The list of short codes that match the search options; returns <c>null</c> for an unknown organization ID.</returns>
	/// <exception cref="ApiKeyException">Thrown if the API key is not valid.</exception>
	/// <exception cref="HttpStatusCodeException">Thrown if the API returned an unexpected status code.</exception>
	public static Task<PagedResult<ShortCodeResult>?> GetShortCodes(this ILink link, GetShortCodesParams? parms) =>
		Guard.ArgumentNotNull(link).GetShortCodes(parms, default);

	/// <summary>
	/// Gets statistics for a given short code, for the provided date range.
	/// </summary>
	/// <param name="link"/>
	/// <param name="shortCodeId">The short code ID. (Example: <c>"code_686bed403c3991bd676bba4d"</c>)</param>
	/// <param name="startDate">The start date.</param>
	/// <param name="endDate">The end date.</param>
	/// <returns>The statistics for the given short code, if present; returns <c>null</c>
	/// for an unknown short code ID or organization ID.</returns>
	/// <exception cref="ApiKeyException">Thrown if the API key is not valid.</exception>
	/// <exception cref="HttpStatusCodeException">Thrown if the API returned an unexpected status code.</exception>
	public static Task<ShortCodeStatsResult?> GetShortCodeStats(this ILink link, string shortCodeId, DateTimeOffset startDate, DateTimeOffset endDate) =>
		Guard.ArgumentNotNull(link).GetShortCodeStats(shortCodeId, startDate, endDate, default);

	/// <summary>
	/// Gets a list of all the tags used by the organization's short codes.
	/// </summary>
	/// <param name="link"/>
	/// <returns>The combined list of tags, if present; returns <c>null</c> for an unknown organization ID.</returns>
	/// <exception cref="ApiKeyException">Thrown if the API key is not valid.</exception>
	/// <exception cref="HttpStatusCodeException">Thrown if the API returned an unexpected status code.</exception>
	public static Task<string[]?> GetTags(this ILink link) =>
		Guard.ArgumentNotNull(link).GetTags(default);

	/// <summary>
	/// Updates a short code.
	/// </summary>
	/// <param name="link"/>
	/// <param name="shortCodeId">The short code ID. (Example: <c>"code_686bed403c3991bd676bba4d"</c>)</param>
	/// <param name="parms">Parameters describing the changes to be made.</param>
	/// <returns>The updated short code</returns>
	/// <exception cref="ApiKeyException">Thrown if the API key is not valid.</exception>
	/// <exception cref="NotFoundException">Thrown for an unknown short code ID or organization ID.</exception>
	/// <exception cref="HttpStatusCodeException">Thrown if the API returned an unexpected status code.</exception>
	public static Task<ShortCodeResult> UpdateShortCode(this ILink link, string shortCodeId, UpdateShortCodeParams parms) =>
		Guard.ArgumentNotNull(link).UpdateShortCode(shortCodeId, parms, default);
}
