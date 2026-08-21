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

public class FakeLogger(FakeLogCollector collector, LoggerExternalScopeProvider scopeProvider) : ILogger
{
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => scopeProvider.Push(state);

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        var scopes = new List<object?>();
        scopeProvider.ForEachScope((scope, list) => list.Add(scope), scopes);

        collector.Add(new FakeLogRecord(logLevel, formatter(state, exception), scopes));
    }
}

public class FakeLoggerProvider(FakeLogCollector collector) : ILoggerProvider
{
    // LoggerExternalScopeProvider is the same AsyncLocal-backed scope stack ASP.NET Core's
    // built-in Console/EventLog loggers use, so concurrent requests each see their own
    // logical scope chain instead of racing on shared mutable state.
    private readonly LoggerExternalScopeProvider _scopeProvider = new();

    public ILogger CreateLogger(string categoryName) => new FakeLogger(collector, _scopeProvider);
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
