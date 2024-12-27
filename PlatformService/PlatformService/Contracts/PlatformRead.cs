using System.ComponentModel.DataAnnotations;

namespace PlatformService.Contracts
{
    public sealed class PlatformRead
    {
        public required int Id {get;set;}
        public required string Name { get; set; }
        public required string Publisher { get; set; }
        public required string Cost { get; set; }
        public Guid ExternalId { get; set; }
    }
}
