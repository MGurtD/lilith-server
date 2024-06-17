using Lilith.Server.Contracts.StatusRequest;
using Lilith.Server.Contracts;
using Lilith.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lilith.Server.Controllers;

public class StatusController(IStatusService statusService) : ControllerBase
{
    private readonly IStatusService _statusService = statusService;

    [HttpPost("/Status/Open")]
    public async Task<IActionResult> Open([FromBody] StatusRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }
        var result = await _statusService.SetStatusToWorkcenter(request.StatusId, request.WorkcenterId);
        if (!result)
        {
            return BadRequest();
        }
        return Ok(new GenericResponse<bool>(true));
    }
}
