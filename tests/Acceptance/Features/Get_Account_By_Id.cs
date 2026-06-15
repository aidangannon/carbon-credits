using System.Net;
using Acceptance.CommonSteps;
using Core.Errors;
using LightBDD.Framework.Scenarios;
using LightBDD.XUnit3;
using Microsoft.Extensions.Logging;

namespace Acceptance.Features;

public partial class Get_Account_By_Id
{
    [Scenario]
    public Task When_Account_Does_Not_Exist_Not_Found_Should_Be_Returned()
    {
        _accountId = Guid.NewGuid();

        return Runner.RunScenarioAsync(
            when => Get_Account_By_Id_ID_Request_Is_Sent(_accountId),
            then => HttpSteps.The_Response_Body_Should_Be_Message_MESSAGE("Not found", _httpResponse!),
            and => HttpSteps.The_Response_Should_Have_Problem_Details_With_Status_STATUS_And_Detail_DETAIL(HttpStatusCode.NotFound, AccountErrors.NotFound, _httpResponse!),
            and => LogSteps.There_Should_Be_A_Log_With_Level_LEVEL_And_Message_MESSAGE_And_Scopes_SCOPES(LogLevel.Information, EndpointCalledMessage, _scopes, _services),
            and => LogSteps.There_Should_Be_A_Log_With_Level_LEVEL_And_Message_MESSAGE_And_Scopes_SCOPES(LogLevel.Information, EndpointCompletedMessage, _scopes, _services)
        );
    }

    [Scenario]
    public Task When_Account_Exists_Account_Should_Be_Returned()
    {
        return Runner.RunScenarioAsync(
            given => An_Account_Exists(),
            when => Get_Account_By_Id_ID_Request_Is_Sent(_accountId),
            then => HttpSteps.The_Response_Should_Have_Status_Code_STATUS_CODE(HttpStatusCode.OK, _httpResponse!),
            and => The_Response_Equals_Account(),
            and => LogSteps.There_Should_Be_A_Log_With_Level_LEVEL_And_Message_MESSAGE_And_Scopes_SCOPES(LogLevel.Information, EndpointCalledMessage, _scopes, _services),
            and => LogSteps.There_Should_Be_A_Log_With_Level_LEVEL_And_Message_MESSAGE_And_Scopes_SCOPES(LogLevel.Information, EndpointCompletedMessage, _scopes, _services)
        );
    }
}
