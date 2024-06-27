namespace Lilith.Server.Entities;

public class Operator
{
    public Guid WorkcenterId { get; set; }
    public Guid OperatorId { get; set; }
    public required string OperatorCode { get; set; }
    public required string OperatorName { get; set; }
    public required string OperatorTypeName { get; set; }
    public required string OperatorTypeDescription { get; set; }
    public DateTime StartTime {  get; set; }
    public DateTime EndTime { get; set; }
}
