using DataAccess.Entities;
using DataAccess.Models.MemberShipUsage;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.MappingConfigs
{
    public class MemberShipUsageMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<MemberShipUsage, MemberShipUsageDTO>()
                .Map(dest => dest.Status, src => src.Status.ToString()) // enum → string
                .Map(dest => dest.CustomerName, src => src.Customer != null ? src.Customer.FullName : null)
                .Map(dest => dest.PackageName, src => src.MembershipPackage != null ? src.MembershipPackage.Name : null);
        }
    }
}
