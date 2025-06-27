using DataAccess.Entities;
using DataAccess.Models.MembershipPackage;
using DataAccess.Models.MemberShipPackage;
using Mapster;

namespace DataAccess.MappingConfigs;

public class MembershipPackageMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<MembershipPackage, MembershipPackageDTO>()
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Description, src => src.Description) 
            .Map(dest => dest.Price, src => src.Price)
            .Map(dest => dest.DurationInDays, src => src.DurationInDays)
            .Map(dest => dest.IsActive, src => src.IsActive)
            .Map(dest => dest.CreatedAt, src => src.CreatedAt)
            .Map(dest => dest.UpdatedAt, src => src.UpdatedAt); 

        config.NewConfig<MembershipPackageDTO, MembershipPackage>()
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Description, src => src.Description)  
            .Map(dest => dest.Price, src => src.Price)
            .Map(dest => dest.DurationInDays, src => src.DurationInDays)
            .Map(dest => dest.IsActive, src => src.IsActive)
            .Map(dest => dest.CreatedAt, src => src.CreatedAt)
            .Map(dest => dest.UpdatedAt, src => src.UpdatedAt);

        config.NewConfig<CreateMembershipPackageDTO, MembershipPackage>()
            .Ignore(dest => dest.Id)
            .Map(dest => dest.CreatedAt, src => DateTime.UtcNow)
            .Map(dest => dest.UpdatedAt, src => DateTime.UtcNow); 
    }
}
