using AutoMapper;
using PlatformService.Contracts;
using PlatformService.Models;

namespace PlatformService.Profiles
{
    public class PlatformProfile : Profile
    {
        public PlatformProfile()
        {
            CreateMap<Platform, PlatformRead>();
            CreateMap<PlatformCreate, Platform>();
        }
    }
}
