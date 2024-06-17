using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lilith.Server.Contracts.Requests;

public class WorkOrderPhaseRequest
{
    public Guid WorkcenterId { get; set; }
    public Guid WorkOrderPhaseId { get; set; }    
}
