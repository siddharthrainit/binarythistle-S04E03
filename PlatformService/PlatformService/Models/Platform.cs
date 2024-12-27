using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace PlatformService.Models
{
    public sealed class Platform
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required string Name { get; set; }

        [Required]
        public required string Publisher { get; set; }

        [Required]
        public required string Cost { get; set; }

        public Guid ExternalId { get; set; }
    }
}
