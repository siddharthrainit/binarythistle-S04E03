namespace CommandsService.Dtos
{
    public sealed class CommandReadDto
    {
        public int Id { get; set; }

        public required string HowTo { get; set; }

        public required string CommandLine { get; set; }
    }
}
