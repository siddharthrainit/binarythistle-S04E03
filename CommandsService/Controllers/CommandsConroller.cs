using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers;

[Route("api/commands/platforms/{platformId}/[controller]")]
[ApiController]
public class CommandsController : ControllerBase
{
    private readonly ICommandRepository _commandRepository;
    private readonly IMapper _mapper;

    public CommandsController(ICommandRepository commandRepository, IMapper mapper)
    {
        _commandRepository = commandRepository;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetCommandsForPlatformAsync(int platformId)
    {
        if (platformId <= 0)
        {
            return BadRequest("Platform Id is required");
        }

        Console.WriteLine($"--> Getting Commands for Platform: {platformId} from Command Service");
        if (!await _commandRepository.PlatformExistsAsync(platformId))
        {
            return NotFound();
        }

        var commands = await _commandRepository.GetCommandsForPlatformAsync(platformId);
        var commandReadDtos = _mapper.Map<IEnumerable<CommandReadDto>>(commands);

        Console.WriteLine($"--> returned Commands for Platform: {platformId} from Command Service");

        return Ok(commandReadDtos);
    }

    [HttpGet("{commandId}", Name = "GetCommandForPlatform")]
    public async Task<IActionResult> GetCommandForPlatformAsync(int platformId, int commandId)
    {
        if (platformId <= 0)
        {
            return BadRequest("Platform Id is required");
        }

        if (commandId <= 0)
        {
            return BadRequest("Command Id is required");
        }

        Console.WriteLine(
            $"--> Getting Command: {commandId} for Platform: {platformId} from Command Service"
        );

        if (!await _commandRepository.PlatformExistsAsync(platformId))
        {
            return NotFound();
        }

        var command = await _commandRepository.GetCommandAsync(platformId, commandId);
        if (command == null)
        {
            return NotFound();
        }

        var commandReadDto = _mapper.Map<CommandReadDto>(command);

        Console.WriteLine(
            $"--> returned Command: {commandId} for Platform: {platformId} from Command Service"
        );

        return Ok(commandReadDto);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCommandForPlatformAsync(
        int platformId,
        CommandCreateDto commandCreateDto
    )
    {
        if (platformId <= 0)
        {
            return BadRequest("Platform Id is required");
        }

        if (commandCreateDto == null)
        {
            return BadRequest("CommandCreateDto is required");
        }

        Console.WriteLine($"--> Creating Command for Platform: {platformId} from Command Service");

        if (!await _commandRepository.PlatformExistsAsync(platformId))
        {
            return NotFound();
        }

        var command = _mapper.Map<Command>(commandCreateDto);
        await _commandRepository.CreateCommandAsync(platformId, command);
        await _commandRepository.SaveChangesAsync();

        var commandReadDto = _mapper.Map<CommandReadDto>(command);

        Console.WriteLine(
            $"--> Created Command: {commandReadDto.Id} for Platform: {platformId} from Command Service"
        );

        return CreatedAtRoute(
            "GetCommandForPlatform",
            new { platformId, commandId = commandReadDto.Id },
            commandReadDto
        );
    }
}
