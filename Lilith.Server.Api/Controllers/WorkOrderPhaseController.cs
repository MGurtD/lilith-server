using Lilith.Server.Contracts;
using Lilith.Server.Contracts.Requests;
using Lilith.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lilith.Server.Controllers;

public class WorkOrderPhaseController(IWorkOrderPhaseService workOrderPhaseService) : ControllerBase
{
    private readonly IWorkOrderPhaseService _workOrderPhaseService = workOrderPhaseService;

    [HttpPost("/WorkOrderPhase/Open")]
    public async Task<IActionResult> Open([FromBody] WorkOrderPhaseRequest request)
    {
        if(!ModelState.IsValid)
        {
            return BadRequest();
        }
        var result = await _workOrderPhaseService.SetPhaseToWorkcenter(request.WorkOrderPhaseId, request.WorkcenterId);
        if (!result)
        {
            return BadRequest();
        }
        return Ok(new GenericResponse<bool>(true));
    }

    [HttpPost("/WorkOrderPhase/Close")]
    public async Task<IActionResult> Close([FromBody] WorkOrderPhaseRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }
        var result = await _workOrderPhaseService.UnsetPhaseFromWorkcenter(request.WorkOrderPhaseId, request.WorkcenterId);
        if (!result)
        {
            return BadRequest();
        }
        return Ok(new GenericResponse<bool>(true));
    }
}
