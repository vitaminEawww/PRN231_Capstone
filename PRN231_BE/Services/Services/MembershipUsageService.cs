using DataAccess.Common;
using DataAccess.Models.MemberShipUsage;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Repositories.IRepositories;
using Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class MembershipUsageService : IMembershipUsage
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public MembershipUsageService(IUnitOfWork unitOfWork , IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<ApiResponse> GetMemberShipUsageAsync(int userId)
        {
            try
            {
                var usage = await _unitOfWork.MemberShipUsages
                                              .AsQueryable()
                                              .Include(m => m.Customer) 
                                              .Include(m => m.MembershipPackage)
                                              .FirstOrDefaultAsync(m => m.CustomerId == userId);

                if (usage == null)
                {
                    return new ApiResponse
                    {
                        IsSuccess = false,
                        StatusCode = HttpStatusCode.NotFound,
                        ErrorMessages = new List<string> { "No membership usage found for this user" }
                    };
                }

                return new ApiResponse
                {
                    IsSuccess = true,
                    StatusCode = HttpStatusCode.OK,
                    ErrorMessages = new List<string>(), 
                    Result = new
                    {
                        MemberShipUsageDTO = usage.Adapt<MemberShipUsageDTO>(),
                        customerName = usage.Customer?.FullName,
                        packageName = usage.MembershipPackage?.Name
                    }
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
}
