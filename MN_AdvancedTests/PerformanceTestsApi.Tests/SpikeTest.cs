using FluentAssertions;
using NBomber.CSharp;
using NBomber.Http.CSharp;
using Xunit;

namespace PerformanceTestsApi.Tests;

[Collection("Test collection")]
public class SpikeTest
{
    [Fact]
    public async Task api_should_handle_spike_200_requests()
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
                Simulation.Inject(rate: 200, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(30)) // Utrzymanie stałego obciążenia na poziomie 200 użytkowników);
            );
        
        var stats = NBomberRunner
            .RegisterScenarios(scenario)
            .Run();

        stats.AllOkCount.Should().BeGreaterThan(500);
    }
}