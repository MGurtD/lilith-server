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
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation($"Executing function at: {DateTime.Now}");

            // Create a scope to access scoped services
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                // Retrieve the scoped service within the scope
                var operatorService = scope.ServiceProvider.GetRequiredService<IOperatorService>();

                // Use the scoped service
                await operatorService.ClockIn(Guid.NewGuid(), Guid.NewGuid());
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

