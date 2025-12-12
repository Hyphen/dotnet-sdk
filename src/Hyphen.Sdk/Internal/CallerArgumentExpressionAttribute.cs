// Compiler polyfill for .NET Framework

#if NETSTANDARD

#pragma warning disable IDE0130 // Namespace does not match folder structure

namespace System.Runtime.CompilerServices;

[AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
internal sealed class CallerArgumentExpressionAttribute(string parameterName) : Attribute
{
	public string ParameterName { get; } = parameterName;
}

#endif
