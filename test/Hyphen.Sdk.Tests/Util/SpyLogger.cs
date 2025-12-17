namespace Microsoft.Extensions.Logging;

internal class SpyLogger<T> : ILogger<T>
{
	public readonly List<string> Messages = [];

	public IDisposable? BeginScope<TState>(TState state)
		where TState : notnull
	{
		Messages.Add($"Scope started: {state}");

		return new ScopeDisposalLogger<TState>(Messages, state);
	}

	public bool IsEnabled(LogLevel logLevel) => true;

	public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
	{
		if (eventId != default)
			Messages.Add($"[{logLevel}::{eventId}] {formatter(state, exception)}");
		else
			Messages.Add($"[{logLevel}] {formatter(state, exception)}");
	}

	class ScopeDisposalLogger<TState>(List<string> messages, TState state) : IDisposable
		where TState : notnull
	{
		public void Dispose() => messages.Add($"Scope disposed: {state}");
	}
}
