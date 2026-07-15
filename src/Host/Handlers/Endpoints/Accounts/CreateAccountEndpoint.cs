using Application.Slces.Accounts;
using FluentValidation;
using Host.Constants;
using Host.Extensions;
using Host.Mappers.Accounts;
using Host.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Host.Handlers.Endpoints.Accounts;

public static class CreateAccountEndpoint
{
    public static RouteGroupBuilder MapCreateAccount(this RouteGroupBuilder group)
    {
        group
            .MapPost("", CreateAccount)
            .WithSummary("Creates an account")
            .WithDescription("Creates a new account with the given name");

        return group;
    }

    private static async Task<Results<Created<AccountResponse>, ValidationProblem, ProblemHttpResult>> CreateAccount(
        [FromBody] CreateAccountRequest request,
        [FromServices] IValidator<CreateAccountRequest> validator,
        [FromServices] IAccountCreationService accountCreationService,
        [FromServices] ILoggerFactory loggerFactory,
        CancellationToken cancellationToken
    )
    {
        var logger = loggerFactory.CreateLogger(LoggingOperations.CreateAccount);
        using var _ = logger.BeginScope(new Dictionary<string, object>
        {
            [Operation] = LoggingOperations.CreateAccount,
            [AccountName] = request.Name
        });

        logger.LogInformation("Endpoint Called");

        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        var account = request.ToAccount();
        var serviceResult = await accountCreationService.CreateAccount(account, cancellationToken);

        logger.LogInformation("Endpoint Completed");

        if (serviceResult.HasFailed())
        {
            return serviceResult.ToProblemResult();
        }

        var accountResponse = serviceResult.Unwrap().ToResponse();

        return TypedResults.Created($"/accounts/{accountResponse.Id}", accountResponse);
    }
}
