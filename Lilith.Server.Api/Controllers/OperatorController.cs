using Microsoft.AspNetCore.Mvc;
using Lilith.Server.Contracts;
using Lilith.Server.Services;
using Lilith.Server.Contracts.Requests;

namespace Lilith.Server.Controllers;

public class OperatorController(IOperatorService operatorService) : ControllerBase
{
    private readonly IOperatorService _operatorService = operatorService;

    [HttpPost("/Operator/ClockIn")]
    public async Task<IActionResult> ClockIn([FromBody] OperatorRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }
        var result = await _operatorService.SetOperatorToWorkcenter(request.OperatorId, request.WorkcenterId);
        return Ok(new GenericResponse<bool>(result));
    }

    [HttpPost("/Operator/ClockOut")]
    public async Task<IActionResult> ClockOut([FromBody] OperatorRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }
        var result = await _operatorService.UnsetOperatorFromWorkcenter(request.OperatorId, request.WorkcenterId);
        return Ok(new GenericResponse<bool>(result));
    }

}
