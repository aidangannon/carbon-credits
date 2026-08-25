using Acceptance;
using Acceptance.Infrastructure;
using LightBDD.Core.Configuration;
using LightBDD.XUnit3;
using Xunit.v3;

[assembly: TestPipelineStartup(typeof(ConfiguredLightBddScope))]

namespace Acceptance;

public class ConfiguredLightBddScope : LightBddScope
{
    protected override void OnConfigure(LightBddConfiguration configuration)
    {
    }

    protected override void OnSetUp()
    {
        TestWebApplicationFactory.Instance = new TestWebApplicationFactory();

        // force ioc container to build deps chain before tests run
        _ = TestWebApplicationFactory.Instance.Server;
    }

    protected override void OnTearDown()
    {
        TestWebApplicationFactory
            .Instance?
            .DisposeAsync()
            .GetAwaiter()
            .GetResult();
    }
}
