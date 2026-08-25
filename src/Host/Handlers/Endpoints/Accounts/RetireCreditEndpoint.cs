using Application.Slices.Accounts;
using Host.Constants;
using Host.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Host.Handlers.Endpoints.Accounts;

public static class RetireCreditEndpoint
{
    public static RouteGroupBuilder MapRetireCredit(this RouteGroupBuilder group)
    {
        group
            .MapDelete("{accountId:guid}/credits/{creditId:guid}", RetireCredit)
            .WithSummary("Retires a credit on an account")
            .WithDescription("Marks the given credit on the given account as retired, enforcing account and credit issuance invariants");

        return group;
    }

    private static async Task<Results<NoContent, ProblemHttpResult>> RetireCredit(
        [FromRoute] Guid accountId,
        [FromRoute] Guid creditId,
        [FromServices] ICreditRetirementService creditRetirementService,
        [FromServices] ILoggerFactory loggerFactory,
        CancellationToken cancellationToken
    )
    {
        var logger = loggerFactory.CreateLogger(LoggingOperations.RetireCredit);
        using var _ = logger.BeginScope(new Dictionary<string, object>
        {
            [Operation] = LoggingOperations.RetireCredit,
            [AccountId] = accountId.ToString(),
            [CreditId] = creditId.ToString()
        });

        logger.LogInformation("Endpoint Called");

        var serviceResult = await creditRetirementService.RetireCredit(accountId, creditId, cancellationToken);

        logger.LogInformation("Endpoint Completed");

        if (serviceResult.HasFailed())
        {
            return serviceResult.ToProblemResult();
        }

        return TypedResults.NoContent();
    }
}
