using System.Diagnostics.CodeAnalysis;

namespace Hyphen.Sdk;

/// <summary>
/// Represents a location returned from <see cref="INetInfo"/>.
/// </summary>
public class Location
{
	/// <summary>
	/// Gets or sets the location's city
	/// </summary>
	[JsonPropertyName("city")]
	[ExcludeFromCodeCoverage]
	public required string City { get; set; }

	/// <summary>
	/// Gets or sets the location's country.
	/// </summary>
	[JsonPropertyName("country")]
	[ExcludeFromCodeCoverage]
	public required string Country { get; set; }

	/// <summary>
	/// Gets or sets the location's geographic name ID.
	/// </summary>
	[JsonPropertyName("geonameId")]
	[ExcludeFromCodeCoverage]
	public required int GeoNameId { get; set; }

	/// <summary>
	/// Gets or sets the location's latitude.
	/// </summary>
	[JsonPropertyName("lat")]
	[ExcludeFromCodeCoverage]
	public required decimal Latitude { get; set; }

	/// <summary>
	/// Gets or sets the location's longitude.
	/// </summary>
	[JsonPropertyName("lng")]
	[ExcludeFromCodeCoverage]
	public required decimal Longitude { get; set; }

	/// <summary>
	/// Gets or sets the location's postal code.
	/// </summary>
	[JsonPropertyName("postalCode")]
	[ExcludeFromCodeCoverage]
	public required string PostalCode { get; set; }

	/// <summary>
	/// Gets or sets the location's region.
	/// </summary>
	[JsonPropertyName("region")]
	[ExcludeFromCodeCoverage]
	public required string Region { get; set; }

	/// <summary>
	/// Gets or sets the location's time zone.
	/// </summary>
	[JsonPropertyName("timezone")]
	[ExcludeFromCodeCoverage]
	public required string TimeZone { get; set; }
}
