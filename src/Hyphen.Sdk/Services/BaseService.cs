using Hyphen.Sdk.Internal;

namespace Hyphen.Sdk;

/// <summary>
/// Represents a base class used for all service classes.
/// </summary>
/// <param name="logger">The logger to be used by the service class</param>
public abstract class BaseService(ILogger logger)
{
	/// <summary>
	/// Gets the logger used to log messages.
	/// </summary>
#pragma warning disable CA1051 // This is instance-visible so it can be used for code-generated logging extensions
	protected readonly ILogger Logger = Guard.ArgumentNotNull(logger);
#pragma warning restore CA1051
}
