using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using PlatformService.Data;
using PlatformService.Repositories;
using PlatformService.Repositories.Implementation;
using PlatformService.SyncDataService.Http;
using PlatformService.SyncDataService.Http.Implementation;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var webHostEnvironment =builder.Environment;
        var service = builder.Services;
        var configuration = builder.Configuration;

        // Add services to the container.
       service.AddControllers();
       service.AddEndpointsApiExplorer();
       service.AddSwaggerGen();
       
        if(webHostEnvironment.IsProduction())
        {
            Console.WriteLine($"--> initializing sql server {configuration.GetConnectionString("PlatformConn")}");
            service.AddDbContext<AppDbContext>(op=>op.UseSqlServer(configuration.GetConnectionString("PlatformConn")));
            Console.WriteLine("--> initialized sql server");
        }
        else
        {
            Console.WriteLine("--> using in memory database");
            service.AddDbContext<AppDbContext>(op=>op.UseSqlServer(configuration.GetConnectionString("PlatformConn")));
            //builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("InMem"));
        }
        
       service.AddScoped<IPlatformRepository, PlatformRepository>();
       service.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();
       service.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        
        Console.WriteLine($"--> CommandService Endpoint {builder.Configuration["CommandService"]}");

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();

        app.MapControllers();        

        PrepDb.PrepPopulation(app, webHostEnvironment.IsProduction());
        app.Run();
    }
}