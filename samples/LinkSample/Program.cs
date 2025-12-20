using Hyphen.Sdk;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddLink();

// Configure these if they aren't available via environment variable
builder.Services.Configure<LinkOptions>(options =>
{
	//options.ApiKey = "";
	//options.OrganizationId = "";
});

var host = builder.Build();

var link = host.Services.GetRequiredService<ILink>();
var tags = await link.GetTags();

if (tags is null)
	Console.WriteLine("Could not find tags. Is your organization ID correct?");
else
{
	Console.WriteLine("Your short code tags:");

	foreach (var tag in tags.OrderBy(x => x))
		Console.WriteLine("* {0}", tag);
}
