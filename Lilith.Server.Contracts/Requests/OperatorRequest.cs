using System.ComponentModel.DataAnnotations;

namespace Lilith.Server.Contracts.Requests;

public class OperatorRequest
{
    [Required]
    public Guid OperatorId { get; set; }
    [Required]
    public Guid WorkcenterId { get; set; }
}
