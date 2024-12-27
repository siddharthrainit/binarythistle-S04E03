using PlatformService.Data;
using PlatformService.Models;

namespace PlatformService.Repositories.Implementation
{
    public class PlatformRepository : IPlatformRepository
    {
        private readonly AppDbContext _context;

        public PlatformRepository(AppDbContext context)
        {
            _context = context;
        }
        public void CreatePlatform(Platform platform)
        {
            ArgumentNullException.ThrowIfNull(platform);

            if (platform.ExternalId == Guid.Empty)
            {
                platform.ExternalId = Guid.NewGuid();
            }

            _context.Platforms.Add(platform);
        }

        public Platform? Get(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            return _context.Platforms.FirstOrDefault(p => p.Id == id);
        }

        public Platform? GetById(Guid externalId)
        {
            if (externalId == Guid.Empty)
            {
                throw new ArgumentException(nameof(externalId));
            }

            return _context.Platforms.FirstOrDefault(p => p.ExternalId == externalId);
        }

        public IEnumerable<Platform> GetAll()
        {
            return _context.Platforms.ToList();
        }

        public int GetId(Guid externalId) => _context.Platforms.FirstOrDefault(p => p.ExternalId == externalId)?.Id ?? 0;

        public bool SaveChanges()
        {
            return (_context.SaveChanges()) >= 0;
        }
    }
}
