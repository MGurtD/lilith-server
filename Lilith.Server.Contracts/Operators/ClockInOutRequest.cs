using System.ComponentModel.DataAnnotations;

namespace Lilith.Server.Contracts.Operators;

public class ClockInOutRequest
{
    [Required]
    public Guid OperatorId { get; set; }
    [Required]
    public Guid WorkcenterId { get; set; }
}
