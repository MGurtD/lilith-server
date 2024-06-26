using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lilith.Server.Contracts.Responses;

public class OperatorResponse
{
    public Guid Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string Surname {  get; set; }
    public Guid OperatorTypeId { get; set; }

}
