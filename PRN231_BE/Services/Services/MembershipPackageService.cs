using DataAccess.Common;
using DataAccess.Entities;
using DataAccess.Models.MembershipPackage;
using DataAccess.Models.MemberShipPackage;
using Mapster;
using MapsterMapper;
using Repositories.IRepositories;
using Services.IServices;
using System.Net;


namespace Services.Services;

public class MembershipPackageService : IMembershipPackageService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public MembershipPackageService( IUnitOfWork unitOfWork, IMapper mapper)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }
    public async Task<ApiResponse> GetAllMembershipPackagesAsync()
    {
        try
        {
            var packages = await _unitOfWork.MembershipPackages.GetAllAsync();

            var packagesDto = packages.Select(pkg => pkg.Adapt<MembershipPackageDTO>()).ToList();

            return new ApiResponse
            {
                IsSuccess = true,
                StatusCode = HttpStatusCode.OK,
                Result = packagesDto
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse
            {
                IsSuccess = false,
                StatusCode = HttpStatusCode.InternalServerError,
                ErrorMessages = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse> GetMembershipPackageByIdAsync(int id)
    {
        try
        {
            var package = await _unitOfWork.MembershipPackages.GetByIdAsync(id);
            if (package == null)
            {
                return new ApiResponse
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.NotFound,
                    ErrorMessages = new List<string> { "Package not found." }
                };
            }

            var packageDto = package.Adapt<MembershipPackageDTO>();
            return new ApiResponse
            {
                IsSuccess = true,
                StatusCode = HttpStatusCode.OK,
                Result = packageDto
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse
            {
                IsSuccess = false,
                StatusCode = HttpStatusCode.InternalServerError,
                ErrorMessages = new List<string> { ex.Message }
            };
        }
    }
    public async Task<ApiResponse> CreateMembershipPackageAsync(CreateMembershipPackageDTO packageDto)
    {
        try
        {
            var package = _mapper.Map<MembershipPackage>(packageDto);
            package.CreatedAt = DateTime.UtcNow;
            package.UpdatedAt = DateTime.UtcNow;


            await _unitOfWork.MembershipPackages.AddAsync(package);
            await _unitOfWork.SaveAsync();

            var result = _mapper.Map<MembershipPackageDTO>(package);

            return new ApiResponse
            {
                IsSuccess = true,
                StatusCode = HttpStatusCode.Created,
                Result = package 
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse
            {
                IsSuccess = false,
                StatusCode = HttpStatusCode.InternalServerError,
                ErrorMessages = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse> GetPagedMembershipPackagesAsync(int pageNumber, int pageSize)
    {
        try
        {
            var packagesQuery = _unitOfWork.MembershipPackages.AsQueryable();
            var pagedList = await PagedList<MembershipPackage>.CreateAsync(packagesQuery, pageNumber, pageSize);
            var packageDtos = pagedList.Select(pkg => pkg.Adapt<MembershipPackageDTO>()).ToList();

            var result = new
            {
                CurrentPage = pagedList.CurrentPage,
                TotalPages = pagedList.TotalPages,
                PageSize = pagedList.PageSize,
                TotalCount = pagedList.TotalCount,
                Items = packageDtos
            };

            return new ApiResponse
            {
                IsSuccess = true,
                StatusCode = HttpStatusCode.OK,
                Result = result 
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse
            {
                IsSuccess = false,
                StatusCode = HttpStatusCode.InternalServerError,
                ErrorMessages = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse> UpdateMembershipPackageAsync(int id, UpdateMembershipPackageDTO packageDto)
    {
        try
        {
            var package = await _unitOfWork.MembershipPackages.GetByIdAsync(id);

            if (package == null)
            {
                return new ApiResponse
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.NotFound,
                    ErrorMessages = new List<string> { "Package not found" }
                };
            }
            package = packageDto.Adapt(package);
            package.UpdatedAt = DateTime.UtcNow; 

            _unitOfWork.MembershipPackages.Update(package);  
            await _unitOfWork.SaveAsync(); 

            return new ApiResponse
            {
                IsSuccess = true,
                StatusCode = HttpStatusCode.OK,
                Result = package.Adapt<MembershipPackageDTO>()  
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse
            {
                IsSuccess = false,
                StatusCode = HttpStatusCode.InternalServerError,
                ErrorMessages = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse> DeleteMembershipPackageAsync(int id)
    {
        try
        {
            var package = await _unitOfWork.MembershipPackages.GetByIdAsync(id);

            if (package == null)
            {
                return new ApiResponse
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.NotFound,
                    ErrorMessages = new List<string> { "Package not found" }
                };
            }

            _unitOfWork.MembershipPackages.Delete(package);
            await _unitOfWork.SaveAsync();

            return new ApiResponse
            {
                IsSuccess = true,
                StatusCode = HttpStatusCode.NoContent,
                Result = "Package deleted successfully"
            };
        }   
        catch (Exception ex)
        {
            return new ApiResponse
            {
                IsSuccess = false,
                StatusCode = HttpStatusCode.InternalServerError,
                ErrorMessages = new List<string> { ex.Message }
            };
        }
    }

}
