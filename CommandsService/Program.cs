using CommandsService.AsyncDataServices;
using CommandsService.Data;
using CommandsService.Data.Implementation;
using CommandsService.EventProcessing;
using CommandsService.SyncDataServices.Grpc;
using CommandsService.SyncDataServices.Grpc.Implementation;
using Microsoft.EntityFrameworkCore;

namespace CommandsService;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        ConfigureServices(builder.Services, builder.Configuration);
        ConfigureWebApplication(builder.Build());
    }

    private static void ConfigureWebApplication(WebApplication app)
    {
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();
        PrepDb.PrepPopulation(app);
        app.MapControllers();
        app.Run();
    }

    private static void ConfigureServices(IServiceCollection services, ConfigurationManager _config)
    {
        services.AddDbContext<AppDbContext>(op =>
        {
            op.UseInMemoryDatabase("InMem");
        });

        services.AddScoped<ICommandRepository, CommandRepository>();
        services.AddControllers();

        services.AddHostedService<MessageBusSubscriber>();

        services.AddSingleton<IEventProcessor, EventProcessor>();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        services.AddScoped<IPlatformDataClient, PlatformDataClient>();
    }
}
