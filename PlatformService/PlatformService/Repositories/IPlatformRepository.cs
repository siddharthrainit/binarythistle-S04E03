using PlatformService.Models;

namespace PlatformService.Repositories
{
    public interface IPlatformRepository
    {
        bool SaveChanges();

        IEnumerable<Platform> GetAll();

        int GetId(Guid externalId);

        Platform? Get(int id);

        Platform? GetById(Guid externalId);

        void CreatePlatform(Platform platform);
    }
}
