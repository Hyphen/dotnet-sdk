namespace Hyphen.Sdk;

/// <summary>
/// A Hyphen service which can manage short codes.
/// </summary>
public interface ILink
{
	/// <summary>
	/// Creates a QR code which points at a short code link.
	/// </summary>
	/// <param name="shortCodeId">The short code ID. (Example: <c>"code_686bed403c3991bd676bba4d"</c>)</param>
	/// <param name="parms">Optional parameters for creating the QR code.</param>
	/// <param name="cancellationToken">The cancellation token to cancel the request early</param>
	/// <returns>Returns the QR code that was created.</returns>
	/// <exception cref="ApiKeyException">Thrown if the API key is not valid.</exception>
	/// <exception cref="NotFoundException">Thrown for an unknown short code ID or organization ID.</exception>
	/// <exception cref="HttpStatusCodeException">Thrown if the API returned an unexpected status code.</exception>
	Task<QrCodeResult> CreateQrCode(string shortCodeId, CreateQrCodeParams? parms, CancellationToken cancellationToken);

	/// <summary>
	/// Creates a short code for a long URL.
	/// </summary>
	/// <param name="longUrl">The long URL to shorten.</param>
	/// <param name="domain">The domain to use for the short code.</param>
	/// <param name="parms">Optional parameters for creating the short code.</param>
	/// <param name="cancellationToken">The cancellation token to cancel the request early</param>
	/// <returns>Returns the short code that was created.</returns>
	/// <exception cref="ApiKeyException">Thrown if the API key is not valid.</exception>
	/// <exception cref="NotFoundException">Thrown for an unknown organization ID.</exception>
	/// <exception cref="HttpStatusCodeException">Thrown if the API returned an unexpected status code.</exception>
	Task<ShortCodeResult> CreateShortCode(Uri longUrl, string domain, CreateShortCodeParams? parms, CancellationToken cancellationToken);

	/// <summary>
	/// Deletes a QR code.
	/// </summary>
	/// <param name="shortCodeId">The short code ID. (Example: <c>"code_686bed403c3991bd676bba4d"</c>)</param>
	/// <param name="qrCodeId">The QR code ID. (Example: <c>"lqr_66fc51fe144cf3a1bd2a35b1"</c>)</param>
	/// <param name="cancellationToken">The cancellation token to cancel the request early</param>
	/// <exception cref="ApiKeyException">Thrown if the API key is not valid.</exception>
	/// <exception cref="HttpStatusCodeException">Thrown if the API returned an unexpected status code.</exception>
	Task DeleteQrCode(string shortCodeId, string qrCodeId, CancellationToken cancellationToken);

	/// <summary>
	/// Deletes a short code.
	/// </summary>
	/// <param name="shortCodeId">The short code ID. (Example: <c>"code_686bed403c3991bd676bba4d"</c>)</param>
	/// <param name="cancellationToken">The cancellation token to cancel the request early</param>
	/// <exception cref="ApiKeyException">Thrown if the API key is not valid.</exception>
	/// <exception cref="HttpStatusCodeException">Thrown if the API returned an unexpected status code.</exception>
	Task DeleteShortCode(string shortCodeId, CancellationToken cancellationToken);

	/// <summary>
	/// Gets details about a short code.
	/// </summary>
	/// <param name="shortCodeId">The short code ID. (Example: <c>"code_686bed403c3991bd676bba4d"</c>)</param>
	/// <param name="qrCodeId">The QR code ID. (Example: <c>"lqr_66fc51fe144cf3a1bd2a35b1"</c>)</param>
	/// <param name="cancellationToken">The cancellation token to cancel the request early</param>
	/// <returns>Returns the QR code, if present; returns <c>null</c> for an unknown short code ID
	/// or QR code ID or organization ID.</returns>
	/// <exception cref="ApiKeyException">Thrown if the API key is not valid.</exception>
	/// <exception cref="HttpStatusCodeException">Thrown if the API returned an unexpected status code.</exception>
	Task<QrCodeResult?> GetQrCode(string shortCodeId, string qrCodeId, CancellationToken cancellationToken);

	/// <summary>
	/// Gets a list of QR codes associated with a short code.
	/// </summary>
	/// <param name="shortCodeId">The short code ID. (Example: <c>"code_686bed403c3991bd676bba4d"</c>)</param>
	/// <param name="parms">Optional parameters for getting the list of QR codes.</param>
	/// <param name="cancellationToken">The cancellation token to cancel the request early</param>
	/// <returns>The list of QR codes that match the search options; returns <c>null</c> for an unknown
	/// short code ID or organization ID.</returns>
	/// <exception cref="ApiKeyException">Thrown if the API key is not valid.</exception>
	/// <exception cref="HttpStatusCodeException">Thrown if the API returned an unexpected status code.</exception>
	Task<PagedResult<QrCodeResult>?> GetQrCodes(string shortCodeId, GetQrCodesParams? parms, CancellationToken cancellationToken);

	/// <summary>
	/// Gets a short code.
	/// </summary>
	/// <param name="shortCodeId">The short code ID. (Example: <c>"code_686bed403c3991bd676bba4d"</c>)</param>
	/// <param name="cancellationToken">The cancellation token to cancel the request early</param>
	/// <returns>Returns the short code, if available; returns <c>null</c> for an unknown code or organization ID.</returns>
	/// <exception cref="ApiKeyException">Thrown if the API key is not valid.</exception>
	/// <exception cref="HttpStatusCodeException">Thrown if the API returned an unexpected status code.</exception>
	Task<ShortCodeResult?> GetShortCode(string shortCodeId, CancellationToken cancellationToken);

	/// <summary>
	/// Gets a list of short codes.
	/// </summary>
	/// <param name="parms">Optional parameters for getting the list of short codes.</param>
	/// <param name="cancellationToken">The cancellation token to cancel the request early</param>
	/// <returns>The list of short codes that match the search options; returns <c>null</c> for an unknown organization ID.</returns>
	/// <exception cref="ApiKeyException">Thrown if the API key is not valid.</exception>
	/// <exception cref="HttpStatusCodeException">Thrown if the API returned an unexpected status code.</exception>
	Task<PagedResult<ShortCodeResult>?> GetShortCodes(GetShortCodesParams? parms, CancellationToken cancellationToken);

	/// <summary>
	/// Gets statistics for a given short code, for the provided date range.
	/// </summary>
	/// <param name="shortCodeId">The short code ID. (Example: <c>"code_686bed403c3991bd676bba4d"</c>)</param>
	/// <param name="startDate">The start date.</param>
	/// <param name="endDate">The end date.</param>
	/// <param name="cancellationToken">The cancellation token to cancel the request early</param>
	/// <returns>The statistics for the given short code, if present; returns <c>null</c>
	/// for an unknown short code ID or organization ID.</returns>
	/// <exception cref="ApiKeyException">Thrown if the API key is not valid.</exception>
	/// <exception cref="HttpStatusCodeException">Thrown if the API returned an unexpected status code.</exception>
	Task<ShortCodeStatsResult?> GetShortCodeStats(string shortCodeId, DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken);

	/// <summary>
	/// Gets a list of all the tags used by the organization's short codes.
	/// </summary>
	/// <param name="cancellationToken">The cancellation token to cancel the request early</param>
	/// <returns>The combined list of tags, if present; returns <c>null</c> for an unknown organization ID.</returns>
	/// <exception cref="ApiKeyException">Thrown if the API key is not valid.</exception>
	/// <exception cref="HttpStatusCodeException">Thrown if the API returned an unexpected status code.</exception>
	Task<string[]?> GetTags(CancellationToken cancellationToken);

	/// <summary>
	/// Updates a short code.
	/// </summary>
	/// <param name="shortCodeId">The short code ID. (Example: <c>"code_686bed403c3991bd676bba4d"</c>)</param>
	/// <param name="parms">Parameters describing the changes to be made.</param>
	/// <param name="cancellationToken">The cancellation token to cancel the request early</param>
	/// <returns>The updated short code</returns>
	/// <exception cref="ApiKeyException">Thrown if the API key is not valid.</exception>
	/// <exception cref="NotFoundException">Thrown for an unknown short code ID or organization ID.</exception>
	/// <exception cref="HttpStatusCodeException">Thrown if the API returned an unexpected status code.</exception>
	Task<ShortCodeResult> UpdateShortCode(string shortCodeId, UpdateShortCodeParams parms, CancellationToken cancellationToken);
}
