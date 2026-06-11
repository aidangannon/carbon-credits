using System.Net;
using Acceptance.CommonSteps;
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
            when => Get_Account_By_Id_ID_Request_Is_Sent( _accountId),
            then => Http.The_Response_Body_Should_Be_Message_MESSAGE("Not found", _httpResponse!),
            and => Http.The_Response_Should_Have_Problem_Details_With_Status_STATUS_And_Detail_DETAIL(HttpStatusCode.NotFound, $"Account could not be found with id '{_accountId}'", _httpResponse!),
            and => Logs.There_Should_Be_A_Log_With_Level_LEVEL_And_Message_MESSAGE_And_Scopes_SCOPES(LogLevel.Information, EndpointCalledMessage, _scopes, _services),
            and => Logs.There_Should_Be_A_Log_With_Level_LEVEL_And_Message_MESSAGE_And_Scopes_SCOPES(LogLevel.Information, EndpointCompletedMessage, _scopes, _services)
        );
    }
}
