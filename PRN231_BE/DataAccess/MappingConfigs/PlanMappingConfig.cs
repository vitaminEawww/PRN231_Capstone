using DataAccess.Entities;
using DataAccess.Models.Plans;
using Mapster;

namespace DataAccess.MappingConfigs
{
    public class PlanMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Plan, PlanDTO>()
                .Map(dest => dest.Phases, src => src.Phases.Adapt<List<PhaseDTO>>())
                .IgnoreNonMapped(true);

            config.NewConfig<PlanCreateDTO, Plan>()
                .Ignore(dest => dest.Id)
                .Ignore(dest => dest.Phases)
                .IgnoreNonMapped(true);

            config.NewConfig<PlanUpdateDTO, Plan>()
                .Ignore(dest => dest.Phases)
                .IgnoreNonMapped(true);

            config.NewConfig<Phase, PhaseDTO>()
                .IgnoreNonMapped(true);

            config.NewConfig<PhaseCreateDTO, Phase>()
                .Ignore(dest => dest.Id)
                .Ignore(dest => dest.CreatedDate)
                .Ignore(dest => dest.EndDate)
                .Ignore(dest => dest.IsCompleted)
                .Ignore(dest => dest.PlanId)
                .IgnoreNonMapped(true);

            config.NewConfig<PhaseUpdateDTO, Phase>()
                .Ignore(dest => dest.CreatedDate)
                .Ignore(dest => dest.EndDate)
                .Ignore(dest => dest.PlanId)
                .IgnoreNonMapped(true);
        }
    }
}