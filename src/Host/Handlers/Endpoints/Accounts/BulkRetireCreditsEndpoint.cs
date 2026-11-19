using Application.Slices.Accounts;
using FluentValidation;
using Host.Constants;
using Host.Extensions;
using Host.Mappers;
using Host.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Crosscutting.Result;

namespace Host.Handlers.Endpoints.Accounts;

public static class BulkRetireCreditsEndpoint
{
    public static RouteGroupBuilder MapBulkRetireCredits(this RouteGroupBuilder group)
    {
        group
            .MapPost("{accountId:guid}/credits/bulk-delete", BulkRetireCredits)
            .WithSummary("Retires multiple credits on an account")
            .WithDescription("Retires all the given credits on the given account, applying the same rules as retiring a single credit. If any credit cannot be retired none are retired and the failing credits are listed in the response");

        return group;
    }

    private static async Task<Results<NoContent, ValidationProblem, ProblemHttpResult>> BulkRetireCredits(
        [FromRoute] Guid accountId,
        [FromBody] BulkRetireCreditsRequest request,
        [FromServices] IValidator<BulkRetireCreditsRequest> validator,
        [FromServices] IBulkCreditRetirementService bulkCreditRetirementService,
        [FromServices] ILoggerFactory loggerFactory,
        CancellationToken cancellationToken
    )
    {
        var logger = loggerFactory.CreateLogger(LoggingOperations.BulkRetireCredits);
        using var _ = logger.BeginScope(new Dictionary<string, object>
        {
            [Operation] = LoggingOperations.BulkRetireCredits,
            [AccountId] = accountId.ToString()
        });

        logger.LogInformation("Endpoint Called");

        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        var result = await bulkCreditRetirementService.RetireCredits(accountId, request.CreditIds, cancellationToken);

        logger.LogInformation("Endpoint Completed");

        if (result.Error is not null)
        {
            return Result.Err(result.Error).ToProblemResult();
        }

        if (result.Failures.Count > 0)
        {
            return TypedResults.Problem(
                statusCode: StatusCodes.Status422UnprocessableEntity,
                title: "Unprocessable entity",
                detail: "One or more credits could not be retired, no credits were retired",
                extensions: new Dictionary<string, object?>
                {
                    ["failures"] = result.Failures.Select(f => new { creditId = f.CreditId, error = f.Error }).ToList()
                }
            );
        }

        return TypedResults.NoContent();
    }
}
