namespace Lilith.Server.Contracts.Responses;

public class OperatorResponse
{
    public required Guid Id { get; set; }
    public required string Code { get; set; }
    public required string Name { get; set; }
    public required string Surname {  get; set; }
    public required Guid OperatorTypeId { get; set; }

}
