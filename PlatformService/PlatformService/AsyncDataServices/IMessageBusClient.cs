using PlatformService.Contracts;

namespace PlatformService.AysncDataServices
{
    public interface IMessageBusClient
    {
        Task PublishNewPlatformAsyc(PlatformPublishedDto platformPublishedDto);
    }
}
