using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lilith.Server.Contracts.Responses;

public class WorkOrderResponse
{
    public string Code {  get; set; }
    public Guid ReferenceId {  get; set; }
}
