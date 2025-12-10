using Hyphen.Sdk;

var builder = Host.CreateApplicationBuilder(args);

// Read configuration from appsettings.json in the build output directory
builder
	.Configuration
	.SetBasePath(AppContext.BaseDirectory)
	.AddJsonFile("appsettings.json", optional: false);

// Add NetInfo service
builder.Services.AddNetInfo();

// Config option 1: Add NetInfo service with settings from appsettings.json
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
var result = await netInfo.GetIPInfo("65.102.191.139");
Console.WriteLine($"ip = {result.IP}, type = {result.Type}, error = {result.ErrorMessage ?? "null"}, location = {JsonSerializer.Serialize(result.Location)}");
