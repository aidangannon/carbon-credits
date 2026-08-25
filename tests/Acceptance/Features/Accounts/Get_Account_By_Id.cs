using System.Net;
using Acceptance.CommonSteps;
using Core.Errors;
using LightBDD.Framework.Scenarios;
using LightBDD.XUnit3;
using Microsoft.Extensions.Logging;

namespace Acceptance.Features.Accounts;

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

    [Scenario]
    public Task When_No_Filters_Are_Specified_Retired_Credits_Should_Be_Included()
    {
        return Runner.RunScenarioAsync(
            given => An_Account_Exists_With_A_Retired_Credit(),
            when => Get_Account_By_Id_ID_Request_Is_Sent(_accountId),
            then => HttpSteps.The_Response_Should_Have_Status_Code_STATUS_CODE(HttpStatusCode.OK, _httpResponse!),
            and => The_Response_Should_Include_The_Credit_ID(_retiredCreditId)
        );
    }

    [Scenario]
    public Task When_No_Filters_Are_Specified_Future_Credits_Should_Be_Excluded()
    {
        return Runner.RunScenarioAsync(
            given => An_Account_Exists_With_A_Future_Credit(),
            when => Get_Account_By_Id_ID_Request_Is_Sent(_accountId),
            then => HttpSteps.The_Response_Should_Have_Status_Code_STATUS_CODE(HttpStatusCode.OK, _httpResponse!),
            and => The_Response_Should_Not_Include_The_Credit_ID(_futureCreditId)
        );
    }

    [Scenario]
    public Task When_Include_Future_Credits_Is_True_Future_Credits_Should_Be_Included()
    {
        return Runner.RunScenarioAsync(
            given => An_Account_Exists_With_A_Future_Credit(),
            when => Get_Account_By_Id_ID_Request_Is_Sent_With_Filters(_accountId, null, true),
            then => HttpSteps.The_Response_Should_Have_Status_Code_STATUS_CODE(HttpStatusCode.OK, _httpResponse!),
            and => The_Response_Should_Include_The_Credit_ID(_futureCreditId)
        );
    }

    [Scenario]
    public Task When_Include_Retired_Credits_Is_False_Retired_Credits_Should_Be_Excluded()
    {
        return Runner.RunScenarioAsync(
            given => An_Account_Exists_With_A_Retired_Credit(),
            when => Get_Account_By_Id_ID_Request_Is_Sent_With_Filters(_accountId, false, null),
            then => HttpSteps.The_Response_Should_Have_Status_Code_STATUS_CODE(HttpStatusCode.OK, _httpResponse!),
            and => The_Response_Should_Not_Include_The_Credit_ID(_retiredCreditId)
        );
    }
}
