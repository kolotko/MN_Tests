namespace Snapshot.Api.Dto;

public class CustomerRequestDto
{
    public required string Name { get; set; }
    public required string SurName { get; set; }
    public required int Age { get; set; }
}