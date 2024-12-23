using Snapshot.Api.Dto;

namespace Snapshot.Api.Services;

public class CustomerService
{
    public CustomerResponseDto CreateCustomer(CustomerRequestDto request)
    {
        return new CustomerResponseDto()
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            SurName = request.SurName,
            Age = request.Age,
            CreateDate = DateTime.Now
        };
    }
    
    public Task<CustomerResponseDto> CustomerException()
    {
        throw new ArgumentException();
    }
}