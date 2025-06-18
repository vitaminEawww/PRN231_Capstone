using DataAccess.Common;
using DataAccess.Entities;
using DataAccess.Enums;
using DataAccess.Models.MembershipPackage;
using DataAccess.Models.MemberShipPackage;
using DataAccess.Models.MemberShipUsage;
using DataAccess.Models.Payment;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Repositories.IRepositories;
using Services.IServices;
using System.Net;


namespace Services.Services;

public class MembershipPackageService : IMembershipPackageService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IVnPayService _vnPayService;

    public MembershipPackageService( IUnitOfWork unitOfWork, IMapper mapper, IVnPayService vnPayService)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _vnPayService = vnPayService;
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

    public async Task<ApiResponse> UpgradePackageAsync(int userId, int newMemberShipPackageId)
    {
        try
        {
            var membershipUsage = await _unitOfWork.MemberShipUsages
                                                .FirstOrDefaultAsync(m => m.CustomerId == userId && m.Status == PackageStatus.Active);

            if (membershipUsage == null)
            {
                return new ApiResponse
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.NotFound,
                    ErrorMessages = new List<string> { "No active membership found for this user." }
                };
            }

            var newPackage = await _unitOfWork.MembershipPackages.GetByIdAsync(newMemberShipPackageId);
            if (newPackage == null)
            {
                return new ApiResponse
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.NotFound,
                    ErrorMessages = new List<string> { "New membership package not found." }
                };
            }

            membershipUsage.MembershipPackageId = newMemberShipPackageId;
            membershipUsage.StartDate = DateTime.UtcNow;
            membershipUsage.EndDate = DateTime.UtcNow.AddDays(newPackage.DurationInDays); // Gia hạn theo gói mới

            _unitOfWork.MemberShipUsages.Update(membershipUsage);
            await _unitOfWork.SaveAsync();

            return new ApiResponse
            {
                IsSuccess = true,
                StatusCode = HttpStatusCode.OK,
                Result = membershipUsage.Adapt<MemberShipUsageDTO>()
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

    public async Task<ApiResponse> ExtendPackageAsync(int userId, int additionalDays)
    {
        try
        {
            var membershipUsage = await _unitOfWork.MemberShipUsages
                                                    .FirstOrDefaultAsync(m => m.CustomerId == userId && m.Status == PackageStatus.Active);

            if (membershipUsage == null)
            {
                return new ApiResponse
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.NotFound,
                    ErrorMessages = new List<string> { "No active membership found for this user." }
                };
            }

            // Gia hạn thời gian sử dụng gói
            membershipUsage.EndDate = membershipUsage.EndDate.AddDays(additionalDays);

            _unitOfWork.MemberShipUsages.Update(membershipUsage);
            await _unitOfWork.SaveAsync();

            return new ApiResponse
            {
                IsSuccess = true,
                StatusCode = HttpStatusCode.OK,
                Result = membershipUsage.Adapt<MemberShipUsageDTO>()
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
