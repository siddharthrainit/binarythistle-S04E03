namespace CommandsService.Dtos;

public sealed class CommandCreateDto
{
    public required string HowTo { get; set; }

    public required string CommandLine { get; set; }
}
