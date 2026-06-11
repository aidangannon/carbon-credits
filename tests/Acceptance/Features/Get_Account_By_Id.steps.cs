using Acceptance.Infrastructure;
using LightBDD.XUnit3;

namespace Acceptance.Features;

public partial class Get_Account_By_Id : FeatureFixture
{
    private Guid _accountId;

    public Get_Account_By_Id()
    {
        TestWebApplicationFactory.Instance!.CreateClient();
    }

    private Task Get_Account_By_Id_ID_Request_Is_Sent()
    {
    }
}
