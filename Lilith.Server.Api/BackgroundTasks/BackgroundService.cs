﻿namespace Lilith.Server.BackgroundTasks;

using Lilith.Server.Entities;
using Lilith.Server.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

public class MyBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<MyBackgroundService> _logger;

    public MyBackgroundService(IServiceScopeFactory serviceScopeFactory, ILogger<MyBackgroundService> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        TimeOnly currentTime = TimeOnly.FromDateTime(DateTime.Now);
        DateTime currentDateTime = DateTime.Now;
        _logger.LogInformation(currentTime.ToString());
        while (!stoppingToken.IsCancellationRequested)
        {
            TimeOnly startTime = TimeOnly.FromDateTime(DateTime.Now);            
            //Data única per tots els centres
            currentTime = TimeOnly.FromDateTime(DateTime.Now);
            currentDateTime = DateTime.Now;
            

            // Create a scope to access scoped services
            using (var scope = _serviceScopeFactory.CreateScope())
            {

                var workcenterService = scope.ServiceProvider.GetRequiredService<IWorkcenterService>();
                var shiftService = scope.ServiceProvider.GetRequiredService<IShiftService>();
                var workcenterDataService = scope.ServiceProvider.GetRequiredService<IWorkcenterDataService>();

                // Access cached workcenters
                var workcenters = await workcenterService.GetAllWorkcenters();
                foreach (var workcenter in workcenters)
                {
                    if((workcenter.ShiftId is null) || (workcenter.ShiftId.Value.Equals("00000000-0000-0000-0000-000000000000")))
                    {
                        continue;
                    }
                    //Recollir el torn teoric pel centre de treball
                    //Comparar, si es el que toca, actualitzar el endTime
                    //Si no es el que toca canviar y posar el shiftstarttime, com a starttime del centre
                    var shiftDetail = await shiftService.GetShiftDetail(workcenter.ShiftId.Value, currentTime);
                    if (shiftDetail != null)
                    {
                        if(shiftDetail.ShiftDetailId == workcenter.ShiftDetailId)
                        {
                            if(!await workcenterService.KeepAliveWorkcenter(workcenter.WorkcenterId, currentDateTime))
                            {
                                _logger.LogInformation("Error keepalive al centre: " + workcenter.WorkcenterName + " Data " + currentDateTime);
                            }        
                            if(!await workcenterDataService.KeepAliveWorkcenterData(workcenter.WorkcenterDataId, currentDateTime))
                            {
                                _logger.LogInformation("Error keepalive al data del centre: " + workcenter.WorkcenterName + " Data " + currentDateTime);
                            }
                        }
                        else
                        {
                            if(!await workcenterService.UpdateWorkcenterShift(workcenter.WorkcenterId,shiftDetail.ShiftDetailId, currentDateTime))
                            {
                                _logger.LogInformation("Error al canviar el torn al centre: " + workcenter.WorkcenterName + " Data " + currentDateTime);
                            }
                            workcenter.ShiftDetailId = shiftDetail.ShiftDetailId;
                            //Tancar registre anterior
                            if (!await workcenterDataService.CloseWorkcenterData(workcenter.WorkcenterDataId, currentDateTime))
                            {
                                _logger.LogInformation("Error al tancar el registre del torn al centre: " + workcenter.WorkcenterName + " Data " + currentDateTime);
                            }
                            //obrir registre nou
                            workcenter.WorkcenterDataId = await workcenterDataService.OpenWorkcenterData(workcenter.WorkcenterId);
                            if (workcenter.WorkcenterDataId < 0)
                            {
                                _logger.LogInformation("Error a l'obrir el registre del torn al centre: " + workcenter.WorkcenterName + " Data " + currentDateTime);
                            }
                            //actualitzar workcenter
                            if (!await workcenterService.SetWorkcenterDataToWorkcenter(workcenter.WorkcenterId, workcenter.WorkcenterDataId))
                            {
                                _logger.LogInformation("Error al setejar el registre del torn al centre: " + workcenter.WorkcenterName + " Data " + currentDateTime);
                            }
                            

                        }
                    }
                }
            }            
            TimeOnly endTime = TimeOnly.FromDateTime(DateTime.Now);
            TimeSpan difference = endTime - startTime;
           _logger.LogInformation($"Execution time: {difference}");
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken); // Delay for 10 seconds
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        // Perform cleanup actions if needed
        _logger.LogInformation("Stopping the background task...");

        await base.StopAsync(cancellationToken);
    }
}

