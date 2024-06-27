using Lilith.Server.Services;

using Lilith.Server.Entities;
using Microsoft.AspNetCore.Mvc;
using Lilith.Server.Contracts;

namespace Lilith.Server.Controllers;

public class WorkcenterController(IWorkcenterService workcenterservice) : ControllerBase
{
    private readonly IWorkcenterService _workcenterService = workcenterservice;

    [HttpGet("Workcenter")]
    public async Task<IActionResult> GetWorkcenters()
    {        
        var result = await _workcenterService.GetAllWorkcenters();
        if(!result.Any())
        {
            return BadRequest();
        }
        return Ok(new GenericResponse<IEnumerable<Workcenter>>(true, result));
    }

    [HttpGet("Workcenter/{id}")]
    public IActionResult GetWorkcenter(Guid id)
    {
        var result = _workcenterService.GetWorkcenterById(id);
        if(result == null)
        {
            return BadRequest();
        }
        return Ok(new GenericResponse<Workcenter>(true, result));
    }
}
