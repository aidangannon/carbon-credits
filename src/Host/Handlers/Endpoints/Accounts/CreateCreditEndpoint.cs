using Application.Slices.Accounts;
using Core.Models;
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
            .MapPost("{accountId:guid}/projects/{projectId:guid}/credits", CreateCredit)
            .WithSummary("Creates a credit on an account")
            .WithDescription("Adds a new credit to the given account for the given project, enforcing ownership and issuance invariants");

        return group;
    }

    private static async Task<Results<Created<CreditResponse>, ProblemHttpResult>> CreateCredit(
        [FromRoute] Guid accountId,
        [FromRoute] Guid projectId,
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
            [AccountId] = accountId.ToString()
        });

        logger.LogInformation("Endpoint Called");

        var credit = new Credit
        {
            Id = Guid.NewGuid(),
            IssuedAt = request.IssuedAt,
            ProjectId = request.ProjectId,
            RetiredAt = null
        };

        var serviceResult = await creditCreationService.CreateCredit(accountId, projectId, credit, cancellationToken);

        logger.LogInformation("Endpoint Completed");

        if (serviceResult.HasFailed())
            return serviceResult.ToProblemResult();

        var creditResponse = credit.ToResponse();

        return TypedResults.Created($"/accounts/{accountId}/credits/{creditResponse.Id}", creditResponse);
    }
}
