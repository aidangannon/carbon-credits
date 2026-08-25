using Application.Slices.Accounts;
using Host.Constants;
using Host.Extensions;
using Host.Mappers.Accounts;
using Host.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Host.Handlers.Endpoints.Accounts;

public static class GetAccountByIdEndpoint
{
    public static RouteGroupBuilder MapGetAccountById(this RouteGroupBuilder application)
    {
        application
            .MapGet("{id:guid}", GetAccountById)
            .WithSummary("Gets account by id")
            .WithDescription("For retieving account by id, reutrning account and its credits, if fails then returns 404. " +
                              "Credits are filtered by the includeRetiredCredits and includeFutureCredits query flags - " +
                              "retired credits are included by default, credits issued in the future are excluded by default.");

        return application;
    }

    private static async Task<Results<Ok<AccountResponse>, ProblemHttpResult>> GetAccountById(
        [FromRoute] Guid id,
        [FromServices] IAccountRetrievalService accountRetrievalService,
        [FromServices] ILoggerFactory loggerFactory,
        CancellationToken cancellationToken,
        [FromQuery] bool includeRetiredCredits = true,
        [FromQuery] bool includeFutureCredits = false
    )
    {
        var logger = loggerFactory.CreateLogger(LoggingOperations.GetAccountById);
        using var _ = logger.BeginScope(new Dictionary<string, object>
        {
            [Operation] = LoggingOperations.GetAccountById,
            [AccountId] = id.ToString()
        });

        logger.LogInformation("Endpoint Called");

        var serviceResult = await accountRetrievalService.GetAccountById(id, includeRetiredCredits, includeFutureCredits, cancellationToken);

        logger.LogInformation("Endpoint Completed");

        if (serviceResult.HasFailed())
        {
            return serviceResult.ToProblemResult();
        }

        var accountResponse = serviceResult.Unwrap().ToResponse();

        return TypedResults.Ok(accountResponse);
    }
}
