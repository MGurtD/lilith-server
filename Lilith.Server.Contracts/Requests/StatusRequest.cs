using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lilith.Server.Contracts.Requests;

public class StatusRequest
{
    public Guid WorkcenterId { get; set; }
    public Guid StatusId { get; set; }
}
