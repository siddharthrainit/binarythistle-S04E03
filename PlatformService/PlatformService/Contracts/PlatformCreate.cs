namespace PlatformService.Contracts
{
    public class PlatformCreate
    {
        public required string Name { get; set; }
        public required string Publisher { get; set; }
        public required string Cost { get; set; }
    }
}
