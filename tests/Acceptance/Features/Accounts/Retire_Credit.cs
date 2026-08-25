using System.Net;
using Acceptance.CommonSteps;
using Core.Errors;
using LightBDD.Framework.Scenarios;
using LightBDD.XUnit3;
using Microsoft.Extensions.Logging;

namespace Acceptance.Features.Accounts;

public partial class Retire_Credit
{
    [Scenario]
    public Task When_Account_Does_Not_Exist_Not_Found_Should_Be_Returned()
    {
        _accountId = Guid.NewGuid();

        return Runner.RunScenarioAsync(
            when => A_Retire_Credit_Request_Is_Sent(_accountId, _creditId),
            then => HttpSteps.The_Response_Should_Have_Problem_Details_With_Status_STATUS_And_Detail_DETAIL(HttpStatusCode.NotFound, AccountErrors.NotFound, _httpResponse!)
        );
    }

    [Scenario]
    public Task When_Account_Was_Created_In_The_Future_Error_Should_Be_Returned()
    {
        _accountCreatedAt = DateTime.UtcNow.AddDays(1);

        return Runner.RunScenarioAsync(
            given => An_Account_Exists(),
            when => A_Retire_Credit_Request_Is_Sent(_accountId, _creditId),
            then => HttpSteps.The_Response_Should_Have_Problem_Details_With_Status_STATUS_And_Detail_DETAIL(HttpStatusCode.UnprocessableEntity, AccountErrors.CreatedInFuture, _httpResponse!)
        );
    }

    [Scenario]
    public Task When_Credit_Does_Not_Exist_On_The_Account_Not_Found_Should_Be_Returned()
    {
        return Runner.RunScenarioAsync(
            given => An_Account_Exists_With_No_Credits(),
            when => A_Retire_Credit_Request_Is_Sent(_accountId, _creditId),
            then => HttpSteps.The_Response_Should_Have_Problem_Details_With_Status_STATUS_And_Detail_DETAIL(HttpStatusCode.NotFound, CreditErrors.NotFound, _httpResponse!)
        );
    }

    [Scenario]
    public Task When_Credit_Was_Issued_In_The_Future_Error_Should_Be_Returned()
    {
        _creditIssuedAt = DateTime.UtcNow.AddDays(1);

        return Runner.RunScenarioAsync(
            given => An_Account_Exists(),
            when => A_Retire_Credit_Request_Is_Sent(_accountId, _creditId),
            then => HttpSteps.The_Response_Should_Have_Problem_Details_With_Status_STATUS_And_Detail_DETAIL(HttpStatusCode.UnprocessableEntity, CreditErrors.IssuedInFuture, _httpResponse!)
        );
    }

    [Scenario]
    public Task When_Credit_Is_Already_Retired_Error_Should_Be_Returned()
    {
        _creditRetiredAt = DateTime.UtcNow.AddDays(-1);

        return Runner.RunScenarioAsync(
            given => An_Account_Exists(),
            when => A_Retire_Credit_Request_Is_Sent(_accountId, _creditId),
            then => HttpSteps.The_Response_Should_Have_Problem_Details_With_Status_STATUS_And_Detail_DETAIL(HttpStatusCode.UnprocessableEntity, CreditErrors.AlreadyRetired, _httpResponse!)
        );
    }

    [Scenario]
    public Task When_A_Valid_Request_Is_Sent_Credit_Should_Be_Retired()
    {
        return Runner.RunScenarioAsync(
            given => An_Account_Exists(),
            when => A_Retire_Credit_Request_Is_Sent(_accountId, _creditId),
            then => HttpSteps.The_Response_Should_Have_Status_Code_STATUS_CODE(HttpStatusCode.OK, _httpResponse!),
            and => The_Response_Should_Reflect_The_Retired_Credit(),
            and => LogSteps.There_Should_Be_A_Log_With_Level_LEVEL_And_Message_MESSAGE_And_Scopes_SCOPES(LogLevel.Information, EndpointCalledMessage, _scopes, _services),
            and => LogSteps.There_Should_Be_A_Log_With_Level_LEVEL_And_Message_MESSAGE_And_Scopes_SCOPES(LogLevel.Information, EndpointCompletedMessage, _scopes, _services)
        );
    }
}
