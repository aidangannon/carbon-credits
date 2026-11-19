using System.Net;
using Acceptance.CommonSteps;
using Core.Errors;
using LightBDD.Framework.Scenarios;
using LightBDD.XUnit3;
using Microsoft.Extensions.Logging;

namespace Acceptance.Features.Accounts;

public partial class Bulk_Retire_Credits
{
    [Scenario]
    public Task When_Account_Does_Not_Exist_Not_Found_Should_Be_Returned()
    {
        _requestedIds = [Guid.NewGuid()];

        return Runner.RunScenarioAsync(
            when => A_Bulk_Retire_Request_Is_Sent(),
            then => HttpSteps.The_Response_Should_Have_Problem_Details_With_Status_STATUS_And_Detail_DETAIL(HttpStatusCode.NotFound, AccountErrors.NotFound, _httpResponse!)
        );
    }

    [Scenario]
    public Task When_No_Credit_Ids_Are_Sent_Bad_Request_Should_Be_Returned()
    {
        _requestedIds = [];

        return Runner.RunScenarioAsync(
            given => An_Account_Exists_With_Credits(),
            when => A_Bulk_Retire_Request_Is_Sent(),
            then => HttpSteps.The_Response_Should_Have_Status_Code_STATUS(HttpStatusCode.BadRequest, _httpResponse!)
        );
    }

    [Scenario]
    public Task When_Account_Was_Created_In_The_Future_Error_Should_Be_Returned()
    {
        _accountCreatedAt = DateTime.UtcNow.AddDays(1);
        var credit = NewCredit(DateTime.UtcNow.AddDays(-1));

        return Runner.RunScenarioAsync(
            given => Credits_Exist(credit),
            when => A_Bulk_Retire_Request_Is_Sent(),
            then => HttpSteps.The_Response_Should_Have_Problem_Details_With_Status_STATUS_And_Detail_DETAIL(HttpStatusCode.UnprocessableEntity, AccountErrors.CreatedInFuture, _httpResponse!),
            and => Credits_Should_Be_Unchanged()
        );
    }

    [Scenario]
    public Task When_Any_Credit_Cannot_Be_Retired_None_Are_Retired_And_Failures_Are_Returned()
    {
        var valid = NewCredit(DateTime.UtcNow.AddDays(-1));
        var future = NewCredit(DateTime.UtcNow.AddDays(1));
        var retired = NewCredit(DateTime.UtcNow.AddDays(-2), DateTime.UtcNow.AddDays(-1));
        var missing = Guid.NewGuid();

        return Runner.RunScenarioAsync(
            given => Credits_Exist(valid, future, retired),
            and => The_Request_Also_Includes_A_Credit_Not_On_The_Account(missing),
            when => A_Bulk_Retire_Request_Is_Sent(),
            then => HttpSteps.The_Response_Should_Have_Problem_Details_With_Status_STATUS_And_Detail_DETAIL(HttpStatusCode.UnprocessableEntity, CreditErrors.BulkRetireFailed, _httpResponse!),
            and => The_Response_Should_List_Exactly_N_Failures(3),
            and => The_Response_Should_List_Failure_For_Credit_With_Error(future.Id, CreditErrors.IssuedInFuture),
            and => The_Response_Should_List_Failure_For_Credit_With_Error(retired.Id, CreditErrors.AlreadyRetired),
            and => The_Response_Should_List_Failure_For_Credit_With_Error(missing, CreditErrors.NotFound),
            and => Credits_Should_Be_Unchanged()
        );
    }

    [Scenario]
    public Task When_All_Credits_Are_Valid_All_Should_Be_Retired()
    {
        var first = NewCredit(DateTime.UtcNow.AddDays(-1));
        var second = NewCredit(DateTime.UtcNow.AddDays(-3));

        return Runner.RunScenarioAsync(
            given => Credits_Exist(first, second),
            when => A_Bulk_Retire_Request_Is_Sent(),
            then => HttpSteps.The_Response_Should_Have_Status_Code_STATUS(HttpStatusCode.NoContent, _httpResponse!),
            and => All_Requested_Credits_Should_Be_Retired(),
            and => LogSteps.There_Should_Be_A_Log_With_Level_LEVEL_And_Message_MESSAGE_And_Scopes_SCOPES(LogLevel.Information, EndpointCalledMessage, _scopes, _services),
            and => LogSteps.There_Should_Be_A_Log_With_Level_LEVEL_And_Message_MESSAGE_And_Scopes_SCOPES(LogLevel.Information, EndpointCompletedMessage, _scopes, _services)
        );
    }
}
