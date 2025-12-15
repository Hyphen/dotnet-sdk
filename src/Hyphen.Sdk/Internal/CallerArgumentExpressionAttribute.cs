// Compiler polyfill for .NET Framework

#if NETSTANDARD

#pragma warning disable IDE0130 // Namespace does not match folder structure

using System.Diagnostics.CodeAnalysis;

namespace System.Runtime.CompilerServices;

[AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
[ExcludeFromCodeCoverage]
internal sealed class CallerArgumentExpressionAttribute(string parameterName) : Attribute
{
	public string ParameterName { get; } = parameterName;
}

#endif
