using System.Text.Json;
using Hyphen.Sdk;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddToggle();
builder.Services.Configure<ToggleOptions>(options =>
{
	// Set this if environment variable HYPHEN_APP_ID is not set
	//options.ApplicationId = "YOUR_APP_ID";

	// Set this if environment variable HYPHEN_PROJECT_PUBLIC_KEY is not set
	//options.ProjectPublicKey = "YOUR_PUBLIC_KEY";
});

var host = builder.Build();

var toggle = host.Services.GetRequiredService<IToggle>();

// Get a value with the default (userless) context
WriteResult(await toggle.Evaluate<bool>("some-boolean-toggle"));

// Get a value with user information in the evaluation context
var parms = new EvaluateParams
{
	Context = new()
	{
		User = new() { Id = "YOUR_USER_ID" },
	},
};
WriteResult(await toggle.Evaluate<CustomObject>("some-object-toggle", parms));

static void WriteResult<T>(ToggleEvaluation<T> result)
{
	Console.WriteLine("{0} = {1}", result.Key, JsonSerializer.Serialize(result.Value));

	if (result.Reason is not null)
		Console.WriteLine("Reason: {0}", result.Reason);
	if (result.Exception is not null)
		Console.WriteLine("Error:  {0}", result.Value);

	Console.WriteLine();
}

// The CustomObject is defined by the schema of the object you return back from the
// toggle evaluation.
class CustomObject
{
	public required string StringProperty { get; set; }
	public required int[] IntArrayProperty { get; set; }
	public required decimal SomeCurrencyValue { get; set; }
}
