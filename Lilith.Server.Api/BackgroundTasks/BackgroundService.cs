namespace Lilith.Server.BackgroundTasks;

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
        TimeSpan startTime = DateTime.Now.TimeOfDay;
        _logger.LogInformation(startTime.ToString());
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation($"Executing function at: {DateTime.Now}");

            // Create a scope to access scoped services
            using (var scope = _serviceScopeFactory.CreateScope())
            {

                var workcenterService = scope.ServiceProvider.GetRequiredService<IWorkcenterService>();

                // Access cached workcenters
                var workcenters = await workcenterService.GetAllWorkcenters();
                foreach (var workcenter in workcenters)
                {
                    if((workcenter.ShiftId is null) || (workcenter.ShiftId.Value.Equals("00000000-0000-0000-0000-000000000000")))
                    {
                        continue;
                    }
                    _logger.LogInformation(workcenter.WorkcenterName.ToString());
                    _logger.LogInformation(workcenter.ShiftStartTime.ToString());
                    _logger.LogInformation(workcenter.ShiftId.ToString());
                    _logger.LogInformation(workcenter.ShiftDetailId.ToString());
                }
            }

            // Your function logic goes here

            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken); // Delay for 10 seconds
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        // Perform cleanup actions if needed
        _logger.LogInformation("Stopping the background task...");

        await base.StopAsync(cancellationToken);
    }
}

