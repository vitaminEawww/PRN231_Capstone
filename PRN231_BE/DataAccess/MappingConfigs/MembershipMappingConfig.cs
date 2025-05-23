using DataAccess.Entities;
using DataAccess.Models.Memberships;
using Mapster;

namespace DataAccess.MappingConfigs
{
    public class MembershipMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            // config.NewConfig<Membership, MembershipDTO>()
            //     .Map(dest => dest.Features, src => src.MemFeatures.Select(mf => mf.Feature.Adapt<FeatureDTO>()))
            //     .IgnoreNonMapped(true);

            // config.NewConfig<MembershipCreateDTO, Membership>()
            //     .Ignore(dest => dest.PlanID)
            //     .Ignore(dest => dest.MemFeatures)
            //     .IgnoreNonMapped(true);

            // config.NewConfig<MembershipUpdateDTO, Membership>()
            //     .Ignore(dest => dest.MemFeatures)
            //     .IgnoreNonMapped(true);
        }
    }
}