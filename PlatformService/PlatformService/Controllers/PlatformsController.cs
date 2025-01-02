using System.Security;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AysncDataServices;
using PlatformService.Contracts;
using PlatformService.Models;
using PlatformService.Repositories;
using PlatformService.SyncDataService.Http;

namespace PlatformService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlatformsController : ControllerBase
    {
        private readonly IPlatformRepository _platformRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<PlatformsController> _logger;
        private readonly ICommandDataClient _commandDataClient;
        private readonly IMessageBusClient _messageBus;

        public PlatformsController(
            IPlatformRepository platformRepository,
            IMapper mapper,
            ILogger<PlatformsController> logger,
            ICommandDataClient commandDataClient,
            IMessageBusClient messageBus
        )
        {
            _platformRepository =
                platformRepository ?? throw new ArgumentNullException(nameof(platformRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _commandDataClient =
                commandDataClient ?? throw new ArgumentNullException(nameof(commandDataClient));
            _messageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));
        }

        [HttpGet]
        public ActionResult<IEnumerable<PlatformRead>> GetPlatforms()
        {
            var platforms = _platformRepository.GetAll();

            return Ok(_mapper.Map<IEnumerable<PlatformRead>>(platforms));
        }

        [HttpGet("{guid}", Name = "GetPlatformById")]
        public ActionResult<PlatformRead> GetPlatformById(Guid guid)
        {
            if (guid == Guid.Empty)
            {
                throw new SecurityException();
            }

            var platformId = _platformRepository.GetId(guid);
            if (platformId <= 0)
            {
                throw new SecurityException();
            }

            var platform = _platformRepository.Get(platformId);

            return Ok(_mapper.Map<PlatformRead>(platform));
        }

        [HttpPost]
        public async Task<ActionResult<PlatformRead>> Platform(PlatformCreate platform)
        {
            var contract = _mapper.Map<Platform>(platform);
            _platformRepository.CreatePlatform(contract);
            _platformRepository.SaveChanges();
            var platformRead = _mapper.Map<PlatformRead>(contract);
            Console.WriteLine("Platform created");
            try
            {
                await _commandDataClient.SendPlatformToCommand(platformRead);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "--> Could not send synchornosuly Data");
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine("Published to RabbitMQ");
            try
            {
                var platformPublishedDto = _mapper.Map<PlatformPublishedDto>(platformRead);
                platformPublishedDto.Event = "Platform_Published";
                await _messageBus.PublishNewPlatformAsyc(platformPublishedDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine("--> Could not send asynchronously");
                Console.WriteLine(ex.Message);
            }

            return CreatedAtRoute(
                nameof(GetPlatformById),
                new { guid = platformRead.ExternalId },
                platformRead
            );
        }
    }
}
