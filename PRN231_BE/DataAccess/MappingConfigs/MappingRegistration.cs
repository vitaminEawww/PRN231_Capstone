using Mapster;

namespace DataAccess.MappingConfigs
{
    public static class MappingRegistration
    {
        public static void RegisterMappings()
        {
            TypeAdapterConfig.GlobalSettings.Scan(typeof(UserMappingConfig).Assembly);
            TypeAdapterConfig.GlobalSettings.Scan(typeof(MembershipPackageMappingConfig).Assembly);
        }
    }
}
