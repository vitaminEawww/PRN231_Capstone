using DataAccess.Entities;
using DataAccess.Models.Features;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.MappingConfigs
{
    public class FeatureMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<FeatureCreateDTO, Feature>();
            config.NewConfig<FeatureUpdateDTO, Feature>();
            config.NewConfig<Feature, FeatureDTO>();
        }
    }
}
