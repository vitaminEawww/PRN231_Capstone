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
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.Name, src => src.Name)
                .Map(dest => dest.Description, src => src.Description)
                .Map(dest => dest.Phases, src => src.Phases.Adapt<List<PhaseDTO>>());

            config.NewConfig<PlanCreateDTO, Plan>()
                .Map(dest => dest.Name, src => src.Name)
                .Map(dest => dest.Description, src => src.Description)
                .Map(dest => dest.DurationDays, src => src.DurationDays)
                .Ignore(dest => dest.Id)
                .Ignore(dest => dest.CreatedDate)
                .Ignore(dest => dest.EndDate)
                .Ignore(dest => dest.IsActive)
                .Ignore(dest => dest.Phases)
                .Ignore(dest => dest.UserPlans);

            config.NewConfig<PlanUpdateDTO, Plan>()
                .Ignore(dest => dest.Phases)
                .IgnoreNonMapped(true);

            config.NewConfig<Phase, PhaseDTO>()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.Name, src => src.Name)
                .Map(dest => dest.Description, src => src.Description)
                .Map(dest => dest.DurationDays, src => src.DurationDays)
                .Map(dest => dest.OrderNumber, src => src.OrderNumber)
                .Map(dest => dest.CreatedDate, src => src.CreatedDate)
                .Map(dest => dest.EndDate, src => src.EndDate)
                .Map(dest => dest.IsCompleted, src => src.IsCompleted)
                .Map(dest => dest.PlanId, src => src.PlanId);

            config.NewConfig<PhaseCreateDTO, Phase>()
                .Map(dest => dest.Name, src => src.Name)
                .Map(dest => dest.Description, src => src.Description)
                .Map(dest => dest.DurationDays, src => src.DurationDays)
                .Map(dest => dest.OrderNumber, src => src.OrderNumber)
                .Ignore(dest => dest.Id)
                .Ignore(dest => dest.CreatedDate)
                .Ignore(dest => dest.EndDate)
                .Ignore(dest => dest.IsCompleted)
                .Ignore(dest => dest.PlanId)
                .Ignore(dest => dest.Plan)
                .Ignore(dest => dest.DailyTasks);

            config.NewConfig<PhaseUpdateDTO, Phase>()
                .Ignore(dest => dest.CreatedDate)
                .Ignore(dest => dest.EndDate)
                .Ignore(dest => dest.PlanId)
                .IgnoreNonMapped(true);
        }
    }
}