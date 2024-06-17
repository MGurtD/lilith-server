﻿using Lilith.Server.Services;

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
        return Ok(new GenericResponse<List<Workcenter>>(true, result));
    }
}