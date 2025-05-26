using DataAccess.Entities;
using DataAccess.Models.MemFeatures;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.MappingConfigs
{
    public class MemFeatureMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<MemFeature, MemFeatureDTO>();
            config.NewConfig<MemFeatureCreateDTO, MemFeature>();
        }
    }
}
