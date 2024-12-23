namespace Snapshot.Api.Dto;

public class CustomerResponseDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string SurName { get; set; }
    public required int Age { get; set; }
    public DateTime CreateDate { get; set; }
}