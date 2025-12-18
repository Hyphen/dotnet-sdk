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
	protected ILogger Logger { get; } = Guard.ArgumentNotNull(logger);
}
