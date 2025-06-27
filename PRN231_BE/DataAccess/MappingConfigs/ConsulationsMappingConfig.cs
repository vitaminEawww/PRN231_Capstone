using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Entities;
using DataAccess.Models.Consultation;
using Mapster;

namespace DataAccess.MappingConfigs
{
    public class ConsulationsMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<CreateConsultationDTO, Consultation>()
                .Ignore(dest => dest.Id)
                .Ignore(dest => dest.CreatedAt)
                .Ignore(dest => dest.UpdatedAt)
                .Ignore(dest => dest.Customer)
                .Ignore(dest => dest.Coach)
                .IgnoreNullValues(true);

            config.NewConfig<Consultation, ConsultationDTO>()
                .IgnoreNullValues(true);
        }
    }
}
