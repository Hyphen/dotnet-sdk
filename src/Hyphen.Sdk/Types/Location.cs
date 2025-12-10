using System.Text.Json.Serialization;

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
	public required string City { get; set; }

	/// <summary>
	/// Gets or sets the location's country.
	/// </summary>
	[JsonPropertyName("country")]
	public required string Country { get; set; }

	/// <summary>
	/// Gets or sets the location's geographic name ID.
	/// </summary>
	[JsonPropertyName("geonameId")]
	public required int GeoNameId { get; set; }

	/// <summary>
	/// Gets or sets the location's latitude.
	/// </summary>
	[JsonPropertyName("lat")]
	public required decimal Latitude { get; set; }

	/// <summary>
	/// Gets or sets the location's longitude.
	/// </summary>
	[JsonPropertyName("lng")]
	public required decimal Longitude { get; set; }

	/// <summary>
	/// Gets or sets the location's postal code.
	/// </summary>
	[JsonPropertyName("postalCode")]
	public required string PostalCode { get; set; }

	/// <summary>
	/// Gets or sets the location's region.
	/// </summary>
	[JsonPropertyName("region")]
	public required string Region { get; set; }

	/// <summary>
	/// Gets or sets the location's time zone.
	/// </summary>
	// TODO: Convert this into TimeZone?
	[JsonPropertyName("timezone")]
	public required string TimeZone { get; set; }
}
