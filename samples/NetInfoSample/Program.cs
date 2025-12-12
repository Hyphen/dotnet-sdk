using Hyphen.Sdk;

var builder = Host.CreateApplicationBuilder(args);

// Read configuration from appsettings.json in the build output directory
builder
	.Configuration
	.SetBasePath(AppContext.BaseDirectory)
	.AddJsonFile("appsettings.json", optional: false);

// Add NetInfo service
builder.Services.AddNetInfo();

// Config option 1: Configure via appsettings.json
builder.Services.Configure<NetInfoOptions>(builder.Configuration.GetSection("NetInfo"));

// Config option 2: Configure via code
builder.Services.Configure<NetInfoOptions>(options =>
{
	// Uncomment the line below and put in your API key if you aren't using the HYPHEN_API_KEY environment variable
	//options.ApiKey = "";
});

using var host = builder.Build();

// Get the NetInfo service
var netInfo = host.Services.GetRequiredService<INetInfo>();

// Get information about a single IP address
var result = await (
	args.Length > 0
		? netInfo.GetIPInfo(args[0])  // Print the IP address passed on the command line...
		: netInfo.GetIPInfo()         // ...or the current public IP, if one wasn't passed
);

Console.WriteLine($"IP address '{result.IP}' is type '{result.Type}'");

if (result.ErrorMessage is not null)
	Console.Error.WriteLine($"Error: {result.ErrorMessage}");

if (result.Location is not null)
{
	Console.WriteLine($"Location: (ID = {result.Location.GeoNameId})");
	Console.WriteLine($"  Latitude:    {result.Location.Latitude}");
	Console.WriteLine($"  Longitude:   {result.Location.Longitude}");
	if (result.Location.Country.Length > 0)
		Console.WriteLine($"  Country:     {result.Location.Country}");
	if (result.Location.Region.Length > 0)
		Console.WriteLine($"  Region:      {result.Location.Region}");
	if (result.Location.City.Length > 0)
		Console.WriteLine($"  City:        {result.Location.City}");
	if (result.Location.PostalCode.Length > 0)
		Console.WriteLine($"  Postal Code: {result.Location.PostalCode}");
	if (result.Location.TimeZone is not null)
		Console.WriteLine($"  Time Zone:   {result.Location.TimeZone}");
}
