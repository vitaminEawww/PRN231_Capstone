using AutoMapper;
using DataAccess.Entities;

namespace Services.MappingConfigs;

public class DailyLogMappingProfile : Profile
{
    public DailyLogMappingProfile()
    {
        CreateMap<DailyLog, DailyLog>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.DailyTaskId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
    }
}