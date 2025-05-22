using DataAccess.Entities;
using DataAccess.Models.Users;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.MappingConfigs
{
    public class UserMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<UserCreateDTO, User>()
                .Map(dest => dest.PasswordHash, src => HashPassword(src.Password))
                .Ignore(dest => dest.UserID)
                .IgnoreNullValues(true);
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}
