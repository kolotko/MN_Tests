using System.Net;
using System.Net.Http.Json;
using Bogus;
using Customers.Api.Contracts.Requests;
using Customers.Api.Contracts.Responses;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Customers.Api.Tests.Integration.CustomerController;

[Collection("Test collection")]
public class CreateCustomerControllerTests : IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly Func<Task> _resetDatabase;
    private readonly Faker<CustomerRequest> _customerGenerator =
        new Faker<CustomerRequest>()
            .RuleFor(x => x.FullName, faker => faker.Person.FullName)
            .RuleFor(x => x.Email, faker => faker.Person.Email)
            .RuleFor(x => x.GitHubUsername, CustomerApiFactory.ValidGithubUser)
            .RuleFor(x => x.DateOfBirth, faker => faker.Person.DateOfBirth.Date);

    public CreateCustomerControllerTests(CustomerApiFactory appFactory)
    {
        _client = appFactory.HttpClient;
        _resetDatabase = appFactory.ResetDatabaseAsync;
    }

    [Fact]
    public async Task Create_CreatesUser_WhenDataIsValid()
    {
        // Arange
        var customer = _customerGenerator.Generate();
        
        // Act
        var response = await _client.PostAsJsonAsync("customers", customer);
        
        // Assert
        var customerResponse = await response.Content.ReadFromJsonAsync<CustomerResponse>();
        customerResponse.Should().BeEquivalentTo(customer);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.ToString().Should().Be($"http://localhost/customers/{customerResponse.Id}");
    }

    [Fact]
    public async Task Create_ReturnsValidationError_WhenEmailIsInvalid()
    {
        // Arange
        const string invalidEmail = "invalid-email";
        var customer = _customerGenerator.Clone()
            .RuleFor(x => x.Email, invalidEmail).Generate();
        
        // Act
        var response = await _client.PostAsJsonAsync("customers", customer);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var error = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        error!.Status.Should().Be(400);
        error!.Title.Should().Be("One or more validation errors occurred.");
        error!.Errors["Email"][0].Should().Be($"{invalidEmail} is not a valid email address");
    }
    
    [Fact]
    public async Task Create_ReturnsValidationError_WhenGitHubUserDoesNotExist()
    {
        // Arange
        const string invalidGitHubUser= "invalid-user-qwsdwdvggvfdgxgbxz";
        var customer = _customerGenerator.Clone()
            .RuleFor(x => x.GitHubUsername, invalidGitHubUser).Generate();
        
        // Act
        var response = await _client.PostAsJsonAsync("customers", customer);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var error = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        error!.Status.Should().Be(400);
        error!.Title.Should().Be("One or more validation errors occurred.");
        error!.Errors["Customer"][0].Should().Be($"There is no GitHub user with username {invalidGitHubUser}");
    }
    
    [Fact]
    public async Task Create_ReturnsInternalServerError_WhenGithubIsThrottled()
    {
        // Arrange
        var customer = _customerGenerator.Clone()
            .RuleFor(x => x.GitHubUsername, CustomerApiFactory.ThrottledUser)
            .Generate();

        // Act
        var response = await _client.PostAsJsonAsync("customers", customer);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _resetDatabase();
}
