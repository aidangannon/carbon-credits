using Host.Constants;
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
            .WithDescription("For retieving account by id, reutrning account and its active credits, if fails then returns 404");

        return application;
    }

    private static async Task<Results<Ok<AccountResponse>, NotFound<string>>> GetAccountById(
        [FromRoute] Guid id,
        [FromServices] ILoggerFactory loggerFactory,
        CancellationToken cancellationToken
    )
    {
        var logger = loggerFactory.CreateLogger(LoggingOperations.GetAccountById);
        using var _ = logger.BeginScope(new Dictionary<string, object>
        {
            [Operation] = LoggingOperations.GetAccountById,
            [AccountId] = id.ToString()
        });

        logger.LogInformation("Started Handler");

        logger.LogInformation("Completed Handler");

        return TypedResults.Ok(new AccountResponse
        {
            Id = Guid.NewGuid(),
            Name = "Blah",
            CreatedAt = DateTime.MinValue,
            Credits = []
        });
    }
}
