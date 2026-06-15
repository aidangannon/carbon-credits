using Acceptance.Infrastructure.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Acceptance.Infrastructure;

public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _baseFilePath =
        $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}/.carbon-credits/test-{Guid.NewGuid()}";

    public static TestWebApplicationFactory? Instance { set; get; }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        OverrideConfiguration();
        SetupDb();

        builder.ConfigureServices(services =>
        {
            var collector = new FakeLogCollector();
            services.AddSingleton(collector);
            services.AddLogging(b => b.AddProvider(new FakeLoggerProvider(collector)));
        });
    }

    public override async ValueTask DisposeAsync()
    {
        TearDownDb();
        await base.DisposeAsync();
    }

    private void SetupDb()
    {
        Directory.CreateDirectory(_baseFilePath + "/accounts");
        Directory.CreateDirectory(_baseFilePath + "/projects");
    }

    private void TearDownDb()
    {
        if (Directory.Exists(_baseFilePath))
        {
            Directory.Delete(_baseFilePath, recursive: true);
        }
    }

    private void OverrideConfiguration()
    {
        Environment.SetEnvironmentVariable("FileOptions__BasePath", _baseFilePath);
        Environment.SetEnvironmentVariable("JwtOptions__Key", "carbon-credits-test-secret-key-32chars!!");
        Environment.SetEnvironmentVariable("JwtOptions__Issuer", "carbon-credits-api");
        Environment.SetEnvironmentVariable("JwtOptions__Audience", "carbon-credits-client");
    }
}
