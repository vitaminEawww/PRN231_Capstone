using DataAccess.Entities;
using Mapster;

namespace DataAccess.MappingConfigs;

public class DailyLogMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<DailyLog, DailyLog>()
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.UserId)
            .Ignore(dest => dest.DailyTaskId)
            .Ignore(dest => dest.CreatedAt)
            .Map(dest => dest.UpdatedAt, src => DateTime.UtcNow)
            .IgnoreNullValues(true);
    }
}