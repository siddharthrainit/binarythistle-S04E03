using CommandsService.Models;
using CommandsService.SyncDataServices.Grpc;

namespace CommandsService.Data;

public static class PrepDb
{
    public static void PrepPopulation(IApplicationBuilder appBuilder)
    {
        using (var serviceScope = appBuilder.ApplicationServices.CreateScope())
        {
            var grpcClient = serviceScope.ServiceProvider.GetService<IPlatformDataClient>();
            var commandRepository = serviceScope.ServiceProvider.GetService<ICommandRepository>();
            if (grpcClient is null)
            {
                throw new InvalidOperationException(nameof(grpcClient));
            }

            if (commandRepository is null)
            {
                throw new InvalidOperationException(nameof(commandRepository));
            }

            var plateforms = grpcClient.ReturnAllPlatforms();
            SeedData(commandRepository, plateforms).GetAwaiter().GetResult();
        }
    }

    private static async Task SeedData(
        ICommandRepository commandRepository,
        IEnumerable<Platform> platforms
    )
    {
        Console.WriteLine($"--> Seeding new platforms... {platforms.Count()}");
        foreach (var platform in platforms)
        {
            if (await commandRepository.ExternalPlatformExistsAsync(platform.PlatformId))
            {
                Console.WriteLine($"--> Platform {platform.Name} already exists.");
                continue;
            }

            await commandRepository.CreatePlatformAsync(platform);
        }
        await commandRepository.SaveChangesAsync();
    }
}
