using Application.Slces;
using Host.Constants;
using Host.Extensions;
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

    private static async Task<Results<Ok<AccountResponse>, ProblemHttpResult>> GetAccountById(
        [FromRoute] Guid id,
        [FromServices] ILoggerFactory loggerFactory,
        [FromServices] IAccountRetrievalService accountRetrievalService,
        CancellationToken cancellationToken
    )
    {
        var logger = loggerFactory.CreateLogger(LoggingOperations.GetAccountById);
        using var _ = logger.BeginScope(new Dictionary<string, object>
        {
            [Operation] = LoggingOperations.GetAccountById,
            [AccountId] = id.ToString()
        });

        logger.LogInformation("Endpoint Called");

        var serviceResult = await accountRetrievalService.GetAccountById(id);

        if (serviceResult.HasFailed())
        {
            return serviceResult.ToProblemResult();
        }

        logger.LogInformation("Endpoint Completed");

        return TypedResults.Ok(new AccountResponse
        {
            Id = Guid.NewGuid(),
            Name = "Blah",
            CreatedAt = DateTime.MinValue,
            Credits = []
        });
    }
}
