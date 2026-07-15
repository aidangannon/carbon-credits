using System.Net;
using Acceptance.CommonSteps;
using LightBDD.Framework.Scenarios;
using LightBDD.XUnit3;
using Microsoft.Extensions.Logging;

namespace Acceptance.Features.Projects;

public partial class Create_Project
{
    [Scenario]
    public Task When_Name_Is_Empty_Validation_Error_Should_Be_Returned()
    {
        _name = string.Empty;

        return Runner.RunScenarioAsync(
            when => A_Create_Project_Request_Is_Sent(_name, _country, _type),
            then => HttpSteps.The_Response_Should_Have_Status_Code_STATUS_CODE(HttpStatusCode.BadRequest, _httpResponse!),
            and => HttpSteps.The_Response_Body_Should_Be_Message_MESSAGE("Name", _httpResponse!)
        );
    }

    [Scenario]
    public Task When_Country_Is_Empty_Validation_Error_Should_Be_Returned()
    {
        _country = string.Empty;

        return Runner.RunScenarioAsync(
            when => A_Create_Project_Request_Is_Sent(_name, _country, _type),
            then => HttpSteps.The_Response_Should_Have_Status_Code_STATUS_CODE(HttpStatusCode.BadRequest, _httpResponse!),
            and => HttpSteps.The_Response_Body_Should_Be_Message_MESSAGE("Country", _httpResponse!)
        );
    }

    [Scenario]
    public Task When_Type_Is_Invalid_Validation_Error_Should_Be_Returned()
    {
        _type = "InvalidType";

        return Runner.RunScenarioAsync(
            when => A_Create_Project_Request_Is_Sent(_name, _country, _type),
            then => HttpSteps.The_Response_Should_Have_Status_Code_STATUS_CODE(HttpStatusCode.BadRequest, _httpResponse!),
            and => HttpSteps.The_Response_Body_Should_Be_Message_MESSAGE("Type", _httpResponse!)
        );
    }

    [Scenario]
    public Task When_A_Valid_Request_Is_Sent_Project_Should_Be_Created()
    {
        return Runner.RunScenarioAsync(
            when => A_Create_Project_Request_Is_Sent(_name, _country, _type),
            then => HttpSteps.The_Response_Should_Have_Status_Code_STATUS_CODE(HttpStatusCode.Created, _httpResponse!),
            and => The_Response_Should_Reflect_The_Create_Request(),
            and => LogSteps.There_Should_Be_A_Log_With_Level_LEVEL_And_Message_MESSAGE_And_Scopes_SCOPES(LogLevel.Information, EndpointCalledMessage, _scopes, _services),
            and => LogSteps.There_Should_Be_A_Log_With_Level_LEVEL_And_Message_MESSAGE_And_Scopes_SCOPES(LogLevel.Information, EndpointCompletedMessage, _scopes, _services)
        );
    }
}
