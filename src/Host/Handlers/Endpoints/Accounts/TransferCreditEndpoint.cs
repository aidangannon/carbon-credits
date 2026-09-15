using Application.Slices.Accounts;
using Host.Constants;
using Host.Extensions;
using Host.Mappers.Accounts;
using Host.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Host.Handlers.Endpoints.Accounts;

public static class TransferCreditEndpoint
{
    public static RouteGroupBuilder MapTransferCredit(this RouteGroupBuilder group)
    {
        group
            .MapPost("{accountId:guid}/credits/{creditId:guid}/transfer", TransferCredit)
            .WithSummary("Transfers a credit to another account")
            .WithDescription("Transfers a credit from one account to another, enforcing project existence and issuance/creation date invariants");

        return group;
    }

    private static async Task<Results<Ok<AccountResponse>, ProblemHttpResult>> TransferCredit(
        [FromRoute] Guid accountId,
        [FromRoute] Guid creditId,
        [FromBody] TransferCreditRequest request,
        [FromServices] ITransferCreditService transferCreditService,
        [FromServices] ILoggerFactory loggerFactory,
        CancellationToken cancellationToken
    )
    {
        var logger = loggerFactory.CreateLogger(LoggingOperations.TransferCredit);
        using var _ = logger.BeginScope(new Dictionary<string, object>
        {
            [Operation] = LoggingOperations.TransferCredit,
            [AccountId] = accountId.ToString(),
            [CreditId] = creditId.ToString(),
            [RecipientAccountId] = request.RecipientAccountId.ToString()
        });

        logger.LogInformation("Endpoint Called");

        var serviceResult = await transferCreditService.TransferCredit(accountId, request.RecipientAccountId, creditId, cancellationToken);

        logger.LogInformation("Endpoint Completed");

        if (serviceResult.HasFailed())
        {
            return serviceResult.ToProblemResult();
        }

        var accountResponse = serviceResult.Unwrap().ToResponse();

        return TypedResults.Ok(accountResponse);
    }
}
