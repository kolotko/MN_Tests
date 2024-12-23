using System.Net.Http.Json;
using Snapshot.Api.Dto;
using Snapshot.Api.Services;

namespace Snapshot.Tests;

public class CreateCustomerEndpointTest
{
    private readonly VerifySettings _verifySettings;
    private readonly HttpClient _client;

    public CreateCustomerEndpointTest()
    {
        _verifySettings = new VerifySettings();
        _verifySettings.UseDirectory("Snapshots");
        _verifySettings.ScrubInlineGuids();
        _verifySettings.ScrubMember("traceparent");
        
        _client = new HttpClient();
    }
    
    [Fact]
    public async Task CreateCustomerTest()
    {
        // Arange
        var request = new CustomerRequestDto()
        {
            Name = "John",
            SurName = "Doe",
            Age = 15
        };
        
        //Act
        var result = await _client.PostAsJsonAsync("http://localhost:5081/customers", request);
        
        //Assert
        await Verify(result, _verifySettings);
    }
}