using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lilith.Server.Contracts.Responses;

public class WorkcenterCostResponse
{
    public Guid Id { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime UpdatedOn { get; set; }
    public bool Disabled { get; set; } = false;
    public Guid WorkcenterId { get; set; }
    public Guid MachineStatusId { get; set; }
    public decimal Cost { get; set; }

}
