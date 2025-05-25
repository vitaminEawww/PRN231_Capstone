using DataAccess.Entities;
using Mapster;

namespace DataAccess.MappingConfigs;

public class DailyTaskMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<DailyTask, DailyTask>()
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.PhaseId)
            .Ignore(dest => dest.CreatedAt)
            .Map(dest => dest.UpdatedAt, src => DateTime.UtcNow)
            .IgnoreNullValues(true);
    }
}