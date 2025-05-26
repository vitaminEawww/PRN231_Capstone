using DataAccess.Common;
using DataAccess.Entities;
using DataAccess.Models.MemFeatures;
using Mapster;
using Repositories.IRepositories;
using Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Services.Service
{
    public class MemFeatureService : IMemFeatureService
    {

        private readonly IUnitOfWork _unitOfWork;

        public MemFeatureService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse> GetAllAsync()
        {
            var response = new ApiResponse();
            try
            {
                var items = await _unitOfWork.MemFeatures.GetAllAsync();
                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.OK;
                response.Result = items.Adapt<List<MemFeatureDTO>>();
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.ErrorMessages.Add(ex.Message);
            }
            return response;
        }

        public async Task<ApiResponse> GetByMembershipIdAsync(int membershipId)
        {
            var response = new ApiResponse();
            try
            {
                var items = _unitOfWork.MemFeatures.AsQueryable()
                    .Where(x => x.MembershipId == membershipId)
                    .ToList();

                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.OK;
                response.Result = items.Adapt<List<MemFeatureDTO>>();
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.ErrorMessages.Add(ex.Message);
            }
            return response;
        }

        public async Task<ApiResponse> CreateAsync(MemFeatureCreateDTO dto)
        {
            var response = new ApiResponse();
            try
            {
                var entity = dto.Adapt<MemFeature>();
                await _unitOfWork.MemFeatures.AddAsync(entity);
                if(entity.MembershipId == null)
                {
                    response.IsSuccess = false;
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.ErrorMessages.Add("MembershipId Not Found");
                    return response;
                }
                await _unitOfWork.SaveAsync();

                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.Created;
                response.Result = entity.Adapt<MemFeatureDTO>();
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.ErrorMessages.Add(ex.Message);
            }
            return response;
        }

        public async Task<ApiResponse> DeleteAsync(int id)
        {
            var response = new ApiResponse();
            try
            {
                var entity = await _unitOfWork.MemFeatures.GetByIdAsync(id);
                if (entity == null)
                {
                    response.IsSuccess = false;
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.ErrorMessages.Add("No Mapping found to delete.");
                    return response;
                }

                _unitOfWork.MemFeatures.Delete(entity);
                await _unitOfWork.SaveAsync();

                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.OK;
                response.Result = "Deleted successfully.";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.ErrorMessages.Add(ex.Message);
            }
            return response;
        }
    }
}
