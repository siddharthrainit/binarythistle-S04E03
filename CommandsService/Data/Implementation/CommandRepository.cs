using CommandsService.Models;
using Microsoft.EntityFrameworkCore;

namespace CommandsService.Data.Implementation
{
    public class CommandRepository : ICommandRepository
    {
        private readonly AppDbContext _context;

        public CommandRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Command>> GetAllCommandsForPlatformAsync(int platformId)
        {
            if (platformId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(platformId));
            }

            return await _context.Commands.Where(c => c.PlatformId == platformId).ToListAsync();
        }

        public async Task<Command> GetCommandAsync(int platformId, int commandId)
        {
            if (platformId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(platformId));
            }

            if (commandId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(commandId));
            }

            return await _context.Commands.FirstOrDefaultAsync(c =>
                    c.PlatformId == platformId && c.Id == commandId
                ) ?? throw new InvalidOperationException("Command not found");
        }

        public async Task CreateCommandAsync(int platformId, Command command)
        {
            if (platformId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(platformId));
            }

            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            command.PlatformId = platformId;
            await _context.Commands.AddAsync(command);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() >= 0;
        }

        public async Task<bool> PlatformExistsAsync(int platformId)
        {
            return await _context.Platforms.AnyAsync(p => p.Id == platformId);
        }

        public async Task<IEnumerable<Platform>> GetAllPlatformsAsync()
        {
            return await _context.Platforms.ToListAsync();
        }

        public async Task CreatePlatformAsync(Platform platform)
        {
            if (platform == null)
            {
                throw new ArgumentNullException(nameof(platform));
            }

            await _context.Platforms.AddAsync(platform);
        }

        public async Task<IEnumerable<Command>> GetCommandsForPlatformAsync(int platformId)
        {
            if (platformId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(platformId));
            }

            return await _context
                .Commands.Where(c => c.PlatformId == platformId)
                .OrderBy(s => s.Platform.Name)
                .ToListAsync();
        }

        public async Task<bool> ExternalPlatformExistsAsync(int externalPlatformId)
        {
            if (externalPlatformId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(externalPlatformId));
            }
            return await _context.Platforms.AnyAsync(p => p.PlatformId == externalPlatformId);
        }
    }
}
