using DataAccess.Entities;
using DataAccess.Models.MemberShips;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.MappingConfigs
{
    public class MembershipMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Membership, MemberShipsDTO>();
            config.NewConfig<MembershipCreateDTO, Membership>();
            config.NewConfig<MemberShipUpdateDTO, Membership>();
        }
    }
}
