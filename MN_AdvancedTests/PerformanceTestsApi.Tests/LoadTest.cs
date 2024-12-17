using FluentAssertions;
using NBomber.CSharp;
using NBomber.Http.CSharp;
using Xunit;

namespace PerformanceTestsApi.Tests;

[Collection("Test collection")]
public class LoadTest
{
    [Fact]
    public async Task api_should_handle_200_requests_per_second()
    {
        var scenario = Scenario.Create("example endpoint", async context =>
            {
                //w celu uniknięcie wysyscenia soketów tworzymy tylko 1 http client
                using var httpClient = new HttpClient();

                var request = Http.CreateRequest("GET", SharedTestCollection.ApiEndpoint)
                    .WithHeader("Content-Type", "application/json");
                var response = await Http.Send(httpClient, request);
                return response;

            }).WithWarmUpDuration(TimeSpan.FromSeconds(5))
            .WithLoadSimulations(
                Simulation.KeepConstant(copies: 100, during: TimeSpan.FromMinutes(10))
            );
        
        var stats = NBomberRunner
            .RegisterScenarios(scenario)
            .Run();

        stats.AllOkCount.Should().BeGreaterThan(500);
    }
}