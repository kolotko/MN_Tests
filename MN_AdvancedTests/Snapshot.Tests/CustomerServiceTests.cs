using Snapshot.Api.Dto;
using Snapshot.Api.Services;

namespace Snapshot.Tests;

public class CustomerServiceTests
{
    private readonly VerifySettings _verifySettings;

    public CustomerServiceTests()
    {
        _verifySettings = new VerifySettings();
        _verifySettings.UseDirectory("Snapshots");
    }
    
    [Fact]
    public async Task CreateCustomerTest()
    {
        // Arange
        var service = new CustomerService();
        var request = new CustomerRequestDto()
        {
            Name = "John",
            SurName = "Doe",
            Age = 15
        };
        
        //Act
        var result = service.CreateCustomer(request);
        
        //Assert
        await Verify(result, _verifySettings);
    }
    
    [Fact]
    public async Task CustomerExceptionTest()
    {
        // Arange
        var service = new CustomerService();
        
        //Act
        var action = () => service.CustomerException();
        
        //Assert
        await ThrowsTask(action, _verifySettings);
    }
}