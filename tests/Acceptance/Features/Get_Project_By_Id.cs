using System.Net;
using Acceptance.CommonSteps;
using Core.Errors;
using LightBDD.Framework.Scenarios;
using LightBDD.XUnit3;
using Microsoft.Extensions.Logging;

namespace Acceptance.Features;

public partial class Get_Project_By_Id
{
    [Scenario]
    public Task When_Project_Does_Not_Exist_Not_Found_Should_Be_Returned()
    {
        _projectId = Guid.NewGuid();

        return Runner.RunScenarioAsync(
            when => Get_Project_By_Id_ID_Request_Is_Sent(_projectId),
            then => HttpSteps.The_Response_Body_Should_Be_Message_MESSAGE("Not found", _httpResponse!),
            and => HttpSteps.The_Response_Should_Have_Problem_Details_With_Status_STATUS_And_Detail_DETAIL(HttpStatusCode.NotFound, ProjectErrors.NotFound, _httpResponse!),
            and => LogSteps.There_Should_Be_A_Log_With_Level_LEVEL_And_Message_MESSAGE_And_Scopes_SCOPES(LogLevel.Information, EndpointCalledMessage, _scopes, _services),
            and => LogSteps.There_Should_Be_A_Log_With_Level_LEVEL_And_Message_MESSAGE_And_Scopes_SCOPES(LogLevel.Information, EndpointCompletedMessage, _scopes, _services)
        );
    }

    [Scenario]
    public Task When_Project_Exists_Project_Should_Be_Returned()
    {
        return Runner.RunScenarioAsync(
            given => A_Project_Exists(),
            when => Get_Project_By_Id_ID_Request_Is_Sent(_projectId),
            then => HttpSteps.The_Response_Should_Have_Status_Code_STATUS_CODE(HttpStatusCode.OK, _httpResponse!),
            and => The_Response_Equals_Project(),
            and => LogSteps.There_Should_Be_A_Log_With_Level_LEVEL_And_Message_MESSAGE_And_Scopes_SCOPES(LogLevel.Information, EndpointCalledMessage, _scopes, _services),
            and => LogSteps.There_Should_Be_A_Log_With_Level_LEVEL_And_Message_MESSAGE_And_Scopes_SCOPES(LogLevel.Information, EndpointCompletedMessage, _scopes, _services)
        );
    }
}
