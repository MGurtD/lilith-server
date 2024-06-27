using System.Text.Json.Serialization;
using Lilith.Server.BackgroundTasks;
using Lilith.Server.Helpers;
using Lilith.Server.Helpers.Database;
using Lilith.Server.Helpers.Middlewares;
using Lilith.Server.Repositories;
using Lilith.Server.Services;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);

var configurationLoader = new ConfigurationLoader();
var configuration = configurationLoader.LoadConfiguration();

// add services to DI container
{
    var services = builder.Services;
    var env = builder.Environment;
 
    services.AddCors();
    services.AddControllers().AddJsonOptions(x =>
    {
        // serialize enums as strings in api responses
        x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

        // ignore omitted parameters on models to enable optional params
        x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });
    services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

    // learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen(options => options.OperationFilter<SecurityRequirementsOperationFilter>());

    // configure DI for application services
    services.AddSingleton(configuration);
    services.AddSingleton<DataContext>();
    services.AddScoped<IOperatorService, OperatorService>();
    services.AddScoped<IWorkcenterRepository, WorkcenterRepository>();
    services.AddScoped<IWorkcenterService, WorkcenterService>();
    services.AddScoped<IShiftRepository, ShiftRepository>();
    services.AddScoped<IShiftService, ShiftService>();
    services.AddScoped<IStatusRepository, StatusRepository>();
    services.AddScoped<IStatusService, StatusService>();
    services.AddScoped<IWorkOrderPhaseRepository, WorkOrderPhaseRepository>();
    services.AddScoped<IWorkOrderPhaseService, WorkOrderPhaseService>();
    services.AddScoped<IOperatorRepository, OperatorRepository>();
    services.AddScoped<IOperatorService, OperatorService>();

    services.AddControllers().AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

    services.AddHttpClient();

    // background services
    services.AddHostedService<MyBackgroundService>();
}

var app = builder.Build();

// ensure database and tables exist
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<DataContext>();
    await context.Init();
    var workcenterService = scope.ServiceProvider.GetRequiredService<IWorkcenterService>();
    await workcenterService.LoadWorkcenterCache();
}

// configure HTTP request pipeline
{    
    app.UseSwagger();
    app.UseSwaggerUI();

    // global cors policy
    app.UseCors(x => x
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());

    // global error handler
    app.UseMiddleware<ErrorHandlerMiddleware>();

    app.MapControllers();
}

app.Run();



