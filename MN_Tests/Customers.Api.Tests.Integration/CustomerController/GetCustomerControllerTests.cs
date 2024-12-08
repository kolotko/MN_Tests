using System.Net;
using System.Net.Http.Json;
using Bogus;
using Customers.Api.Contracts.Requests;
using Customers.Api.Contracts.Responses;
using FluentAssertions;
using Xunit;

namespace Customers.Api.Tests.Integration.CustomerController;

public class GetCustomerControllerTests : IClassFixture<CustomerApiFactory>
{
    private readonly CustomerApiFactory _appFactory;
    private readonly HttpClient _client;
    private readonly Faker<CustomerRequest> _customerGenerator =
        new Faker<CustomerRequest>()
            .RuleFor(x => x.FullName, faker => faker.Person.FullName)
            .RuleFor(x => x.Email, faker => faker.Person.Email)
            .RuleFor(x => x.GitHubUsername, CustomerApiFactory.ValidGithubUser)
            .RuleFor(x => x.DateOfBirth, faker => faker.Person.DateOfBirth.Date);

    public GetCustomerControllerTests(CustomerApiFactory appFactory)
    {
        _appFactory = appFactory;
        _client = _appFactory.CreateClient();
    }
    
    [Fact]
    public async Task Get_ReturnsCustomer_WhenCustomerExists()
    {
        // Arange
        var customer = _customerGenerator.Generate();
        var response = await _client.PostAsJsonAsync("customers", customer);
        var customerResponse = await response.Content.ReadFromJsonAsync<CustomerResponse>();
        
        // Act
        
        var getResponse = await _client.GetAsync($"customers/{customerResponse!.Id}");
        var getCustomerResponse = await getResponse.Content.ReadFromJsonAsync<CustomerResponse>();
        
        // Assert
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        getCustomerResponse.Should().BeEquivalentTo(customerResponse);
    }
    
    [Fact]
    public async Task Get_ReturnsNotFound_WhenCustomerDoesNotExist()
    {
        // Arange
        const string invalidGitHubUser= "invalid-user-id";
        
        // Act
        
        var response = await _client.GetAsync($"customers/{invalidGitHubUser}");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}