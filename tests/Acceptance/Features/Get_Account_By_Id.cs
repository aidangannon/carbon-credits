using LightBDD.Framework.Scenarios;
using LightBDD.XUnit3;

namespace Acceptance.Features;

public partial class Get_Account_By_Id
{
    [Scenario]
    public Task When_Account_Does_Not_Exist_Not_Found_Should_Be_Returned()
    {
        _accountId = Guid.NewGuid();

        return Runner.RunScenarioAsync(
            when => Get_Account_By_Id_ID_Request_Is_Sent( _accountId),
            then => Status_Code_Should_Be(),
            and => Message_Should_Be("")
        );
    }
}
