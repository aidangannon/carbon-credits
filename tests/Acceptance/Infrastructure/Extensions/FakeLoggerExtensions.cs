using Acceptance.Infrastructure.Logging;
using AwesomeAssertions;
using Microsoft.Extensions.Logging;

namespace Acceptance.Infrastructure.Extensions;

public static class FakeLoggerExtensions
{
    public static void ShouldContainLog(
        this FakeLogCollector collector,
        LogLevel level,
        string message,
        params (string Key, string Value)[] scopes)
    {
        var candidates = collector.GetSnapshot()
            .Where(l => l.Level == level && l.Message.Contains(message));

        var match = candidates.FirstOrDefault(l =>
            scopes.All(scope =>
                l.Scopes
                    .OfType<IReadOnlyDictionary<string, object>>()
                    .Any(s => s.TryGetValue(scope.Key, out var v) && v?.ToString() == scope.Value)));

        match.Should().NotBeNull($"expected log [{level}] containing \"{message}\" with scopes [{string.Join(", ", scopes.Select(s => $"{s.Key}={s.Value}"))}] but none was found");
    }

    public static void ShouldNotContainLog(
        this FakeLogCollector collector,
        LogLevel level,
        string message,
        params (string Key, string Value)[] scopes)
    {
        var candidates = collector.GetSnapshot()
            .Where(l => l.Level == level && l.Message.Contains(message));

        var match = candidates.FirstOrDefault(l =>
            scopes.All(scope =>
                l.Scopes
                    .OfType<IReadOnlyDictionary<string, object>>()
                    .Any(s => s.TryGetValue(scope.Key, out var v) && v?.ToString() == scope.Value)));

        match.Should().BeNull($"expected no log [{level}] containing \"{message}\" with scopes [{string.Join(", ", scopes.Select(s => $"{s.Key}={s.Value}"))}] but one was found");
    }
}
