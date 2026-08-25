using System.Net;
using Acceptance.CommonSteps;
using Core.Errors;
using LightBDD.Framework.Scenarios;
using LightBDD.XUnit3;
using Microsoft.Extensions.Logging;

namespace Acceptance.Features.Accounts;

public partial class Create_Credit
{
    [Scenario]
    public Task When_Account_Does_Not_Exist_Not_Found_Should_Be_Returned()
    {
        _accountId = Guid.NewGuid();

        return Runner.RunScenarioAsync(
            when => A_Create_Credit_Request_Is_Sent(_accountId, _issuedAt, _projectId),
            then => HttpSteps.The_Response_Should_Have_Problem_Details_With_Status_STATUS_And_Detail_DETAIL(HttpStatusCode.NotFound, AccountErrors.NotFound, _httpResponse!)
        );
    }

    [Scenario]
    public Task When_Project_Does_Not_Exist_Not_Found_Should_Be_Returned()
    {
        _projectId = Guid.NewGuid();

        return Runner.RunScenarioAsync(
            given => An_Account_Exists(),
            when => A_Create_Credit_Request_Is_Sent(_accountId, _issuedAt, _projectId),
            then => HttpSteps.The_Response_Should_Have_Problem_Details_With_Status_STATUS_And_Detail_DETAIL(HttpStatusCode.NotFound, ProjectErrors.NotFound, _httpResponse!)
        );
    }

    [Scenario]
    public Task When_IssuedAt_Is_In_The_Future_Error_Should_Be_Returned()
    {
        _issuedAt = DateTime.UtcNow.AddDays(1);

        return Runner.RunScenarioAsync(
            given => An_Account_Exists(),
            and => A_Project_Exists_For_The_Credit(),
            when => A_Create_Credit_Request_Is_Sent(_accountId, _issuedAt, _projectId),
            then => HttpSteps.The_Response_Should_Have_Problem_Details_With_Status_STATUS_And_Detail_DETAIL(HttpStatusCode.UnprocessableEntity, CreditErrors.IssuedInFuture, _httpResponse!)
        );
    }

    [Scenario]
    public Task When_A_Valid_Request_Is_Sent_Credit_Should_Be_Added_To_Account()
    {
        return Runner.RunScenarioAsync(
            given => An_Account_Exists(),
            and => A_Project_Exists_For_The_Credit(),
            when => A_Create_Credit_Request_Is_Sent(_accountId, _issuedAt, _projectId),
            then => HttpSteps.The_Response_Should_Have_Status_Code_STATUS(HttpStatusCode.Created, _httpResponse!),
            and => The_Response_Should_Reflect_The_Created_Credit(),
            and => LogSteps.There_Should_Be_A_Log_With_Level_LEVEL_And_Message_MESSAGE_And_Scopes_SCOPES(LogLevel.Information, EndpointCalledMessage, _scopes, _services),
            and => LogSteps.There_Should_Be_A_Log_With_Level_LEVEL_And_Message_MESSAGE_And_Scopes_SCOPES(LogLevel.Information, EndpointCompletedMessage, _scopes, _services)
        );
    }
}
