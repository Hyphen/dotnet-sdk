using Hyphen.Sdk;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddNetInfo(options =>
{
	options.BaseUri = new("https://dev.net.info");
});

using var host = builder.Build();

var netInfo = host.Services.GetRequiredService<INetInfo>();
var result = await netInfo.GetIPInfo("65.102.191.139");
Console.WriteLine($"ip = {result.IP}, type = {result.Type}, error = {result.ErrorMessage ?? "null"}, location = {JsonSerializer.Serialize(result.Location)}");
