using DataAccess.Entities;
using DataAccess.Models.Features;
using Mapster;

namespace DataAccess.MappingConfigs
{
    public class FeatureMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Feature, FeatureDTO>()
                .IgnoreNonMapped(true);

            // config.NewConfig<FeatureCreateDTO, Feature>()
            //     .Ignore(dest => dest.Id)
            //     .Ignore(dest => dest.MemFeatures)
            //     .IgnoreNonMapped(true);

            // config.NewConfig<FeatureUpdateDTO, Feature>()
            //     .Ignore(dest => dest.MemFeatures)
            //     .IgnoreNonMapped(true);

            config.NewConfig<MemFeature, MemFeatureDTO>()
                .Map(dest => dest.Feature, src => src.Feature.Adapt<FeatureDTO>())
                .IgnoreNonMapped(true);
        }
    }
}