using System.Reflection;
using Xunit.v3;

namespace Hyphen.Sdk;

internal sealed class CleanEnvironmentAttribute(params string[] variables)
	: BeforeAfterTestAttribute, ICollectionAttribute
{
	readonly List<(string Name, string? Value)> savedValues = [];

	string ICollectionAttribute.Name => "CleanEnvironmentAttribute";

	Type? ICollectionAttribute.Type => null;

	public override void Before(MethodInfo methodUnderTest, IXunitTest test)
	{
		savedValues.Clear();

		foreach (var variable in variables)
		{
			savedValues.Add((variable, Environment.GetEnvironmentVariable(variable)));
			Environment.SetEnvironmentVariable(variable, null);
		}
	}

	public override void After(MethodInfo methodUnderTest, IXunitTest test)
	{
		foreach (var savedValue in savedValues)
			Environment.SetEnvironmentVariable(savedValue.Name, savedValue.Value);
	}
}
