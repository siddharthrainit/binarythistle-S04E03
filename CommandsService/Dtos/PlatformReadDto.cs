namespace CommandsService.Dtos;

public sealed class PlatformReadDto
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public ICollection<CommandReadDto> Commands { get; set; } = new List<CommandReadDto>();
}
