using Application.Slices.Accounts;
using Host.Constants;
using Host.Extensions;
using Host.Mappers.Accounts;
using Host.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Host.Handlers.Endpoints.Accounts;

public static class CreateCreditEndpoint
{
    public static RouteGroupBuilder MapCreateCredit(this RouteGroupBuilder group)
    {
        group
            .MapPost("{accountId:guid}/credits", CreateCredit)
            .WithSummary("Creates a credit on an account")
            .WithDescription("Adds a new credit to the given account for the given project, enforcing ownership and issuance invariants");

        return group;
    }

    private static async Task<Results<Created<CreditResponse>, ProblemHttpResult>> CreateCredit(
        [FromRoute] Guid accountId,
        [FromBody] CreateCreditRequest request,
        [FromServices] ICreditCreationService creditCreationService,
        [FromServices] ILoggerFactory loggerFactory,
        CancellationToken cancellationToken
    )
    {
        var logger = loggerFactory.CreateLogger(LoggingOperations.CreateCredit);
        using var _ = logger.BeginScope(new Dictionary<string, object>
        {
            [Operation] = LoggingOperations.CreateCredit,
            [AccountId] = accountId.ToString(),
            [ProjectId] = request.ProjectId.ToString()
        });

        logger.LogInformation("Endpoint Called");

        var credit = request.ToCredit();

        var serviceResult = await creditCreationService.CreateCredit(accountId, request.ProjectId, credit, cancellationToken);

        logger.LogInformation("Endpoint Completed");

        if (serviceResult.HasFailed())
        {
            return serviceResult.ToProblemResult();
        }

        var creditResponse = credit.ToResponse();

        return TypedResults.Created($"/accounts/{accountId}/credits/{creditResponse.Id}", creditResponse);
    }
}
