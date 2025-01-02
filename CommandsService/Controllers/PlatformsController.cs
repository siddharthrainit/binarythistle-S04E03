using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers;

[Route("api/commands/[controller]")]
[ApiController]
public class PlatformsController : ControllerBase
{
    private readonly ICommandRepository _commandRepository;
    private readonly IMapper _mapper;

    public PlatformsController(ICommandRepository commandRepository, IMapper mapper)
    {
        _commandRepository = commandRepository;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetPlatformsAsync()
    {
        Console.WriteLine("--> Getting All Platforms from Command Service");

        var platforms = await _commandRepository.GetAllPlatformsAsync();
        var platformReadDtos = _mapper.Map<IEnumerable<PlatformReadDto>>(platforms);

        Console.WriteLine("--> returned All Platforms from Command Service");

        return Ok(platformReadDtos);
    }

    [HttpPost]
    public IActionResult TestInboundConnection()
    {
        Console.WriteLine("--> Inbound POST # Command Service");

        return Ok("Inboud test of from Platforms Controller");
    }
}
