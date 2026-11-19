using System.Net;
using Acceptance.CommonSteps;
using Core.Errors;
using LightBDD.Framework.Scenarios;
using LightBDD.XUnit3;

namespace Acceptance.Features.Accounts;

public partial class Bulk_Retire_Credits
{
    [Scenario]
    public Task When_Account_Does_Not_Exist_Not_Found_Should_Be_Returned()
    {
        return Runner.RunScenarioAsync(
            when => A_Bulk_Retire_Request_Is_Sent(Guid.NewGuid(), new[] { Guid.NewGuid() }),
            then => HttpSteps.The_Response_Should_Have_Problem_Details_With_Status_STATUS_And_Detail_DETAIL(HttpStatusCode.NotFound, AccountErrors.NotFound, _httpResponse!)
        );
    }

    [Scenario]
    public Task When_No_Credit_Ids_Are_Sent_Bad_Request_Should_Be_Returned()
    {
        return Runner.RunScenarioAsync(
            given => An_Account_Exists_With_Credits(),
            when => A_Bulk_Retire_Request_Is_Sent(_accountId, Array.Empty<Guid>()),
            then => HttpSteps.The_Response_Should_Have_Status_Code_STATUS(HttpStatusCode.BadRequest, _httpResponse!)
        );
    }

    [Scenario]
    public Task When_Account_Was_Created_In_The_Future_Error_Should_Be_Returned()
    {
        _accountCreatedAt = DateTime.UtcNow.AddDays(1);

        return Runner.RunScenarioAsync(
            given => An_Account_Exists_With_Credits(),
            when => A_Bulk_Retire_Request_Is_Sent(_accountId, _validIds),
            then => HttpSteps.The_Response_Should_Have_Problem_Details_With_Status_STATUS_And_Detail_DETAIL(HttpStatusCode.UnprocessableEntity, AccountErrors.CreatedInFuture, _httpResponse!)
        );
    }

    [Scenario]
    public Task When_Any_Credit_Cannot_Be_Retired_None_Are_Retired_And_Failures_Are_Reported()
    {
        return Runner.RunScenarioAsync(
            given => An_Account_Exists_With_Credits(),
            when => A_Bulk_Retire_Request_Is_Sent(_accountId, _validIds.Concat(new[] { _futureId, _retiredId, _missingId }).ToArray()),
            then => HttpSteps.The_Response_Should_Have_Status_Code_STATUS(HttpStatusCode.UnprocessableEntity, _httpResponse!),
            and => The_Response_Should_List_The_Failed_Credits(),
            and => The_Valid_Credits_Should_Not_Be_Retired()
        );
    }

    [Scenario]
    public Task When_All_Credits_Are_Valid_They_Should_All_Be_Retired()
    {
        return Runner.RunScenarioAsync(
            given => An_Account_Exists_With_Credits(),
            when => A_Bulk_Retire_Request_Is_Sent(_accountId, _validIds),
            then => HttpSteps.The_Response_Should_Have_Status_Code_STATUS(HttpStatusCode.NoContent, _httpResponse!),
            and => The_Valid_Credits_Should_Be_Retired()
        );
    }
}
