using System.ComponentModel.DataAnnotations;

namespace CommandsService.Models
{
    public sealed class Platform
    {
        [Key]
        public required int Id { get; set; }

        public required int PlatformId { get; set; }

        public required string Name { get; set; }

        public ICollection<Command> Commands { get; set; } = new List<Command>();
    }
}
