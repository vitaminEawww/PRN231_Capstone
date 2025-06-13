using DataAccess.Entities;
using DataAccess.Models.Users;
using Mapster;

namespace DataAccess.MappingConfigs
{
    public class UserMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<UserCreateDTO, User>()
                .Map(dest => dest.PasswordHash, src => HashPassword(src.Password))
                .Ignore(dest => dest.Id)
                .IgnoreNullValues(true);
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}
