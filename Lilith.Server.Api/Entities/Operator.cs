namespace Lilith.Server.Entities;

public class Operator
{
    public Guid WorkcenterId { get; set; }
    public Guid OperatorId { get; set; }
    public string OperatorCode { get; set; }
    public string OperatorName { get; set; }
    public string OperatorTypeName { get; set; }
    public string OperatorTypeDescription { get; set; }
    public DateTime StartTime {  get; set; }
    public DateTime EndTime { get; set; }
}
