using CommandsService.Models;

namespace CommandsService.Data;

public interface ICommandRepository
{
    // Save changes
    Task<bool> SaveChangesAsync();

    // Platform related methods
    Task<IEnumerable<Platform>> GetAllPlatformsAsync();
    Task<bool> PlatformExistsAsync(int platformId);
    Task<bool> ExternalPlatformExistsAsync(int platformId);
    Task CreatePlatformAsync(Platform platform);

    // Command related methods
    Task<IEnumerable<Command>> GetCommandsForPlatformAsync(int platformId);
    Task<Command> GetCommandAsync(int platformId, int commandId);
    Task CreateCommandAsync(int platformId, Command command);
}
