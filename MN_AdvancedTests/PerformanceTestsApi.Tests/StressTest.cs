using System.Text;
using FluentAssertions;
using NBomber.CSharp;
using NBomber.Http.CSharp;
using Xunit;

namespace PerformanceTestsApi.Tests;

[Collection("Test collection")]
public class StressTest
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
                Simulation.RampingConstant(copies: 100,
                    during: TimeSpan.FromSeconds(5)), // Stopniowe zwiększanie do 100 użytkowników
                Simulation.KeepConstant(copies: 200,
                    during: TimeSpan.FromSeconds(5)) // Utrzymanie stałego obciążenia na poziomie 200 użytkowników);
            );
        
        var stats = NBomberRunner
            .RegisterScenarios(scenario)
            .Run();

        stats.AllOkCount.Should().BeGreaterThan(500);
    }
}