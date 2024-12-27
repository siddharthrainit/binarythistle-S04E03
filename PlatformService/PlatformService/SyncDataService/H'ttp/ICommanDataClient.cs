using PlatformService.Contracts;

namespace PlatformService.SyncDataService.Http;

public interface ICommandDataClient
{
    Task SendPlatformToCommand(PlatformRead platformRead);
}