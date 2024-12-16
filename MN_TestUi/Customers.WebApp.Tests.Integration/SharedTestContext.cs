using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Model.Common;
using Ductus.FluentDocker.Services;
using Microsoft.Playwright;
using Xunit;

namespace Customers.WebApp.Tests.Integration;

public class SharedTestContext : IAsyncLifetime
{
    public const string ValidGithubUser = "kolotko";
    public const string AppUrl = "https://localhost:7780";
    private readonly GitHubApiServer _gitHubApiServer = new();
    //                                                                                                 bin\Debug\net8.0
    private static readonly string DockerComposeFile = Path.Combine(Directory.GetCurrentDirectory(), (TemplateString)"../../../docker-compose.integration.yml");
    
    private readonly ICompositeService _dockerService = new Builder()
        .UseContainer()
        .UseCompose()
        .FromFile(DockerComposeFile)
        .RemoveOrphans()
        .WaitForHttp("test-app", "https://localhost:7780")
        .Build();
    
    private IPlaywright _playwright;

    public IBrowser Browser { get; private set; }
    
    public async Task InitializeAsync()
    {
        _gitHubApiServer.Start();
        _gitHubApiServer.SetupUser(ValidGithubUser);
        _dockerService.Start();

        _playwright = await Playwright.CreateAsync();
        Browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            //Headless = false,
            SlowMo = 1000
        });
    }

    public async Task DisposeAsync()
    {
        await Browser.DisposeAsync();
        _playwright.Dispose();
        _dockerService.Dispose();
        _gitHubApiServer.Dispose();
    }
}