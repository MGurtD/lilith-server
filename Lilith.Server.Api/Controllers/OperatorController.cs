using Microsoft.AspNetCore.Mvc;
using Lilith.Server.Contracts;
using Lilith.Server.Services;
using Lilith.Server.Contracts.Operators;

namespace Lilith.Server.Controllers;

public class OperatorController(IOperatorService operatorService) : ControllerBase
{
    private readonly IOperatorService _operatorService = operatorService;

    [HttpPost("ClockIn")]
    public async Task<IActionResult> ClockIn([FromBody] ClockInOutRequest request)
    {
        var result = await _operatorService.ClockIn(request.OperatorId, request.WorkcenterId);
        return Ok(new GenericResponse<bool>(result));
    }

    [HttpPost("ClockOut")]
    public async Task<IActionResult> ClockOut([FromBody] ClockInOutRequest request)
    {
        var result = await _operatorService.ClockOut(request.OperatorId, request.WorkcenterId);
        return Ok(new GenericResponse<bool>(result));
    }

}
