using Customers.Api.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Testcontainers.PostgreSql;
using Xunit;

namespace Customers.Api.Tests.Integration;

public class CustomerApiFactory : WebApplicationFactory<IApiMarker>, IAsyncLifetime
{
    public const string ValidGithubUser = "kolotko";
    public const string ThrottledUser = "throttle";
    
    private readonly PostgreSqlContainer _dbContainer =
        new PostgreSqlBuilder()
            .WithImage("postgres:latest")
            .WithDatabase("mydb")
            .WithUsername("postgres")
            .WithPassword("changeme")
            .WithPortBinding(5555,5432)
            .Build();
    
    private readonly GitHubApiServer _gitHubApiServer = new();
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureLogging(logging =>
        {
            // w celu uniknięcia wysyłania logów na zewnątrz systemu 
            logging.ClearProviders();
        });
        
        builder.ConfigureTestServices(services =>
        {
            var x = _dbContainer.GetConnectionString();
            services.RemoveAll(typeof(IDbConnectionFactory));
            services.AddSingleton<IDbConnectionFactory>(_ =>
                new NpgsqlConnectionFactory(_dbContainer.GetConnectionString()));
                
            services.AddHttpClient("GitHub", httpClient =>
            {
                httpClient.BaseAddress = new Uri(_gitHubApiServer.Url);
                httpClient.DefaultRequestHeaders.Add(
                    HeaderNames.Accept, "application/vnd.github.v3+json");
                httpClient.DefaultRequestHeaders.Add(
                    HeaderNames.UserAgent, $"postgres-{Environment.MachineName}");
            });
        });
    }

    public async Task InitializeAsync()
    {
        _gitHubApiServer.Start();
        _gitHubApiServer.SetupUser(ValidGithubUser);
        _gitHubApiServer.SetupThrottledUser(ThrottledUser);
        await _dbContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        _gitHubApiServer.Dispose();
        await _dbContainer.StopAsync();
    }
}