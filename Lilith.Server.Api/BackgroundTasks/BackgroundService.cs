namespace Lilith.Server.BackgroundTasks;

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
        DateOnly currentDate = DateOnly.FromDateTime(DateTime.Now);
        _logger.LogInformation(currentTime.ToString());
        while (!stoppingToken.IsCancellationRequested)
            try
            {


                {
                    TimeOnly startTime = TimeOnly.FromDateTime(DateTime.Now);
                    //Data única per tots els centres
                    currentTime = TimeOnly.FromDateTime(DateTime.Now);
                    currentDateTime = DateTime.Now;
                    currentDate = DateOnly.FromDateTime(DateTime.Now);


                    // Create a scope to access scoped services
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {

                        var workcenterService = scope.ServiceProvider.GetRequiredService<IWorkcenterService>();
                        var shiftService = scope.ServiceProvider.GetRequiredService<IShiftService>();
                        //var workcenterDataService = scope.ServiceProvider.GetRequiredService<IWorkcenterDataService>();
                        //var hd_workcenterStatusService = scope.ServiceProvider.GetRequiredService<IHD_WorkcenterStatusService>();

                        // Access cached workcenters
                        var workcenters = await workcenterService.GetAllWorkcenters();
                        foreach (var workcenter in workcenters)
                        {
                            if ((workcenter.ShiftId is null) || (workcenter.ShiftId.Value.Equals("00000000-0000-0000-0000-000000000000")))
                            {
                                continue;
                            }
                            //Recollir el torn teoric pel centre de treball
                            //Comparar, si es el que toca, actualitzar el endTime                    
                            //Si no es el que toca canviar y posar el shiftstarttime, com a starttime del centre
                            var shiftDetail = await shiftService.GetCurrentShiftDetail(workcenter.ShiftId.Value, currentTime);
                            //var workcenterstatus = await hd_workcenterStatusService.GetCurrentWorkcenterStatus(workcenter.WorkcenterDataId);
                            if (shiftDetail != null)
                            {
                                DateOnly workcenterDate = DateOnly.FromDateTime(workcenter.CurrentDay);
                                if (shiftDetail.ShiftDetailId == workcenter.ShiftDetailId && workcenterDate.Equals(currentDate))
                                {
                                    if (!await workcenterService.KeepAliveWorkcenter(workcenter.WorkcenterId, currentDateTime))
                                    {
                                        _logger.LogInformation("Error keepalive al centre: " + workcenter.WorkcenterName + " Data " + currentDateTime);
                                    }
                                    workcenter.ShiftEndTime = currentDateTime;
                                    workcenter.CurrentTime = currentDateTime.TimeOfDay;
                                    if(workcenter.StatusName.Length > 0)
                                    {
                                        workcenter.StatusEndTime = currentDateTime;
                                    }
                                    if(workcenter.PhaseCode.Length > 0)
                                    {
                                        workcenter.PhaseEndTime = currentDateTime;
                                    }
                                }
                                else
                                {
                                    var shiftStartTime = shiftDetail.ShiftStartTime;
                                    TimeSpan timeFromShiftStartTime = shiftStartTime.TimeOfDay;
                                    DateTime currentToday = DateTime.Today;
                                    DateTime currentBuildDateTime = currentToday.Add(timeFromShiftStartTime);



                                    //TORN                            
                                    if (!await shiftService.SetShiftDetailToWorkcenter(shiftDetail, workcenter.WorkcenterId))
                                    {
                                        _logger.LogInformation("Error al canviar el torn al centre: " + workcenter.WorkcenterName + " Data " + currentBuildDateTime);
                                    }
                                    workcenter.ShiftDetailId = shiftDetail.ShiftDetailId;




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
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred during background service execution.");
            }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        // Perform cleanup actions if needed
        _logger.LogInformation("Stopping the background task...");

        await base.StopAsync(cancellationToken);
    }
}

