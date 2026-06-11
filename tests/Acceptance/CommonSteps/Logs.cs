using Acceptance.Infrastructure.Extensions;
using Acceptance.Infrastructure.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Acceptance.CommonSteps;

public static class Logs
{
    public static Task There_Should_Be_A_Log_With_Level_LEVEL_And_Message_MESSAGE_And_Scopes_SCOPES(
        LogLevel level,
        string message,
        IDictionary<string, string> scopes,
        IServiceProvider serviceProvider
    )
    {
        var logCollector = serviceProvider.GetRequiredService<FakeLogCollector>();
        logCollector.ShouldContainLog(level, message, [.. scopes.Select(s => (s.Key, s.Value))]);

        return Task.CompletedTask;
    }

    public static Task There_Should_Not_Be_A_Log_With_Level_LEVEL_And_Message_MESSAGE_And_Scopes_SCOPES(
        LogLevel level,
        string message,
        IDictionary<string, string> scopes,
        IServiceProvider serviceProvider
    )
    {
        var logCollector = serviceProvider.GetRequiredService<FakeLogCollector>();
        logCollector.ShouldNotContainLog(level, message, [.. scopes.Select(s => (s.Key, s.Value))]);

        return Task.CompletedTask;
    }
}
