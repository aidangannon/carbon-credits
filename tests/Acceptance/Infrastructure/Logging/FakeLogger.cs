using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Acceptance.Infrastructure.Logging;

public record FakeLogRecord(LogLevel Level, string Message, IReadOnlyList<object?> Scopes);

public class FakeLogCollector
{
    private readonly ConcurrentBag<FakeLogRecord> _logs = [];

    public void Add(FakeLogRecord record) => _logs.Add(record);

    public IReadOnlyList<FakeLogRecord> GetSnapshot() => [.. _logs];
}

public class FakeLogger(FakeLogCollector collector) : ILogger
{
    private static readonly AsyncLocal<List<object?>?> _scopesLocal = new();

    private static List<object?> CurrentScopes
    {
        get
        {
            _scopesLocal.Value ??= [];
            return _scopesLocal.Value;
        }
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        CurrentScopes.Add(state);
        return new ScopeDisposable(() => CurrentScopes.Remove(state));
    }

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        collector.Add(new FakeLogRecord(logLevel, formatter(state, exception), [.. CurrentScopes]));
    }

    private sealed class ScopeDisposable(Action onDispose) : IDisposable
    {
        public void Dispose() => onDispose();
    }
}

public class FakeLoggerProvider(FakeLogCollector collector) : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName) => new FakeLogger(collector);
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
