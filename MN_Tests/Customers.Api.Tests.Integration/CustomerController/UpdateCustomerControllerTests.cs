using System.Net;
using System.Net.Http.Json;
using Bogus;
using Customers.Api.Contracts.Requests;
using Customers.Api.Contracts.Responses;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Customers.Api.Tests.Integration.CustomerController;

public class UpdateCustomerControllerTests : IClassFixture<CustomerApiFactory>
{
    private readonly CustomerApiFactory _appFactory;
    private readonly HttpClient _client;
    private readonly Faker<CustomerRequest> _customerGenerator =
        new Faker<CustomerRequest>()
            .RuleFor(x => x.FullName, faker => faker.Person.FullName)
            .RuleFor(x => x.Email, faker => faker.Person.Email)
            .RuleFor(x => x.GitHubUsername, CustomerApiFactory.ValidGithubUser)
            .RuleFor(x => x.DateOfBirth, faker => faker.Person.DateOfBirth.Date);

    public UpdateCustomerControllerTests(CustomerApiFactory appFactory)
    {
        _appFactory = appFactory;
        _client = _appFactory.CreateClient();
    }
    
    [Fact]
    public async Task Update_UpdatesUser_WhenDataIsValid()
    {
        // Arange
        var customer = _customerGenerator.Generate();
        var response = await _client.PostAsJsonAsync("customers", customer);
        var customerResponse = await response.Content.ReadFromJsonAsync<CustomerResponse>();
        
        var customerToUpdate = _customerGenerator.Generate();
        
        // Act
        
        var putResponse = await _client.PutAsJsonAsync($"customers/{customerResponse!.Id}", customerToUpdate);
        var putCustomerResponse = await putResponse.Content.ReadFromJsonAsync<CustomerResponse>();
        
        // Assert
        putResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        putCustomerResponse.Should().BeEquivalentTo(customerToUpdate);
    }
    
    [Fact]
    public async Task Update_ReturnsValidationError_WhenEmailIsInvalid()
    {
        // Arange
        var customer = _customerGenerator.Generate();
        var response = await _client.PostAsJsonAsync("customers", customer);
        var customerResponse = await response.Content.ReadFromJsonAsync<CustomerResponse>();
        
        const string invalidEmail = "invalid-email";
        var customerToUpdate = _customerGenerator.Clone()
            .RuleFor(x => x.Email, invalidEmail).Generate();
        
        // Act
        var putResponse = await _client.PutAsJsonAsync($"customers/{customerResponse!.Id}", customerToUpdate);
        
        // Assert
        putResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var error = await putResponse.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        error!.Status.Should().Be(400);
        error!.Title.Should().Be("One or more validation errors occurred.");
        error!.Errors["Email"][0].Should().Be($"{invalidEmail} is not a valid email address");
    }
    
    [Fact]
    public async Task Update_ReturnsValidationError_WhenGitHubUserDoestNotExist()
    {
        // Arange
        var customer = _customerGenerator.Generate();
        var response = await _client.PostAsJsonAsync("customers", customer);
        var customerResponse = await response.Content.ReadFromJsonAsync<CustomerResponse>();
        
        const string invalidGitHubUser= "invalid-user-qwsdwdvggvfdgxgbxz";
        var customerToUpdate = _customerGenerator.Clone()
            .RuleFor(x => x.GitHubUsername, invalidGitHubUser).Generate();
        
        // Act
        var putResponse = await _client.PutAsJsonAsync($"customers/{customerResponse!.Id}", customerToUpdate);
        
        // Assert
        putResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var error = await putResponse.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        error!.Status.Should().Be(400);
        error!.Title.Should().Be("One or more validation errors occurred.");
        error!.Errors["Customer"][0].Should().Be($"There is no GitHub user with username {invalidGitHubUser}");
    }
}