using Microsoft.EntityFrameworkCore;
using PlatformService.AysncDataServices;
using PlatformService.AysncDataServices.Implementation;
using PlatformService.Data;
using PlatformService.Repositories;
using PlatformService.Repositories.Implementation;
using PlatformService.SyncDataService.Grpc;
using PlatformService.SyncDataService.Http;
using PlatformService.SyncDataService.Http.Implementation;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var webHostEnvironment = builder.Environment;

        // Add services to the container.
        ConfigureServices(builder.Services, builder.Environment, builder.Configuration);

        Console.WriteLine($"--> CommandService Endpoint {builder.Configuration["CommandService"]}");
        ConfigureWebHostBuilder(builder.Build(), webHostEnvironment);
    }

    private static void ConfigureWebHostBuilder(
        WebApplication webApplication,
        IHostEnvironment webHostEnvironment
    )
    {
        // Configure the HTTP request pipeline.
        if (webApplication.Environment.IsDevelopment())
        {
            webApplication.UseSwagger();
            webApplication.UseSwaggerUI();
        }

        webApplication.UseRouting();
        webApplication.UseAuthorization();
        webApplication.UseEndpoints(endpoints =>
        {
            _ = endpoints.MapControllers();
            _ = endpoints.MapGrpcService<GrpcPlatformService>();
            _ = endpoints.MapGet(
                "/protos/platforms.proto",
                async ctx =>
                {
                    await ctx.Response.WriteAsync(
                        await File.ReadAllTextAsync("Protos/platforms.proto")
                    );
                }
            );
        });

        webApplication.MapControllers();

        PrepDb.PrepPopulation(webApplication, webHostEnvironment.IsProduction());
        webApplication.Run();
    }

    private static void ConfigureServices(
        IServiceCollection service,
        IHostEnvironment hostEnvironment,
        IConfiguration configuration
    )
    {
        service.AddControllers();
        service.AddEndpointsApiExplorer();
        service.AddSwaggerGen();

        if (hostEnvironment.IsProduction())
        {
            Console.WriteLine(
                $"--> initializing sql server {configuration.GetConnectionString("PlatformConn")}"
            );
            service.AddDbContext<AppDbContext>(op =>
                op.UseSqlServer(configuration.GetConnectionString("PlatformConn"))
            );
            Console.WriteLine("--> initialized sql server");
        }
        else
        {
            Console.WriteLine("--> using in memory database");
            service.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("InMem"));
        }

        service.AddScoped<IPlatformRepository, PlatformRepository>();
        service.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();
        service.AddSingleton<IMessageBusClient, MessageBusClient>();
        service.AddGrpc();
        service.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
    }
}
