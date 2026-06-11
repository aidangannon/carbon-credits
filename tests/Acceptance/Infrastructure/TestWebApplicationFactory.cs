using Acceptance.Infrastructure.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Acceptance.Infrastructure;

public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _baseFilePath = $"~/.carbon-credits/test-{Guid.NewGuid()}/";

    public static TestWebApplicationFactory? Instance { set; get; }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        OverrideConfiguration();

        builder.ConfigureServices(services =>
        {
            var collector = new FakeLogCollector();
            services.AddSingleton(collector);
            services.AddLogging(b => b.AddProvider(new FakeLoggerProvider(collector)));
        });
        builder.UseEnvironment("testing");
    }

    public override async ValueTask DisposeAsync()
    {
        if (Directory.Exists(_baseFilePath))
        {
            Directory.Delete(_baseFilePath, recursive: true);
        }

        await base.DisposeAsync();
    }

    private void OverrideConfiguration()
    {
        Environment.SetEnvironmentVariable("File__BasePath", _baseFilePath);
    }
}
