using AutoMapper;
using Grpc.Core;
using PlatformService.Repositories;

namespace PlatformService.SyncDataService.Grpc;

public sealed class GrpcPlatformService : GrpcPlatform.GrpcPlatformBase
{
    private readonly IPlatformRepository _platformRepository;
    private readonly IMapper _mapper;

    public GrpcPlatformService(IPlatformRepository platformRepository, IMapper mapper)
    {
        _platformRepository = platformRepository;
        _mapper = mapper;
    }

    public override Task<PlatformResponse> GetAllPlatforms(
        GetAllRequest request,
        ServerCallContext context
    )
    {
        var response = new PlatformResponse();
        var platforms = _platformRepository.GetAll();

        foreach (var platform in platforms)
        {
            response.Platform.Add(_mapper.Map<GrpcPlatformModel>(platform));
        }

        return Task.FromResult(response);
    }
}
