using AutoMapper;
using CommandsService.Models;
using Grpc.Net.Client;
using PlatformService;

namespace CommandsService.SyncDataServices.Grpc.Implementation
{
    public class PlatformDataClient : IPlatformDataClient
    {
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        public Func<string, string> GetConfigurationValue { get; private set; }

        public PlatformDataClient(IConfiguration configuration, IMapper mapper)
        {
            _configuration = configuration;
            _mapper = mapper;
            GetConfigurationValue = key =>
                _configuration[key] ?? throw new ArgumentNullException(key);
        }

        public IEnumerable<Platform> ReturnAllPlatforms()
        {
            var configName =
                _configuration["GrpcPlatform"] ?? throw new ArgumentNullException("GrpcPlatform");
            Console.WriteLine($"--> Calling gRPC Service to get Platforms {configName}");

            var channel = GrpcChannel.ForAddress(configName);
            var client = new GrpcPlatform.GrpcPlatformClient(channel);

            var request = new GetAllRequest();

            try
            {
                var reply = client.GetAllPlatforms(request);

                return _mapper.Map<IEnumerable<Platform>>(reply.Platform);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not call gRPC Server. {ex.Message}");
            }
            return Array.Empty<Platform>();
        }
    }
}
