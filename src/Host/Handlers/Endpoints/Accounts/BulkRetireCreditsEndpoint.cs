using Application.Slices.Accounts;
using FluentValidation;
using Host.Constants;
using Host.Mappers;
using Host.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Host.Handlers.Endpoints.Accounts;

public static class BulkRetireCreditsEndpoint
{
    public static RouteGroupBuilder MapBulkRetireCredits(this RouteGroupBuilder group)
    {
        group
            .MapPost("{accountId:guid}/credits/bulk-delete", BulkRetireCredits)
            .WithSummary("Bulk retires credits on an account")
            .WithDescription("Retires all the given credits on the given account, applying the same rules as retiring a single credit. If any credit cannot be retired none are retired and the failures are returned");

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
            [AccountId] = accountId.ToString(),
            [CreditIds] = string.Join(",", request.CreditIds)
        });

        logger.LogInformation("Endpoint Called");

        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        var serviceResult = await bulkCreditRetirementService.RetireCredits(accountId, request.CreditIds, cancellationToken);

        logger.LogInformation("Endpoint Completed");

        if (serviceResult.HasFailed())
        {
            var error = ErrorCodeMapper.ToErrorDetails(serviceResult.Error!);
            return TypedResults.Problem(
                statusCode: error.StatusCode,
                title: error.Title,
                detail: serviceResult.Error,
                extensions: new Dictionary<string, object?>
                {
                    ["failures"] = serviceResult.Failures
                        .Select(f => new { creditId = f.CreditId, error = f.Error })
                        .ToList()
                }
            );
        }

        return TypedResults.NoContent();
    }
}
