using DataAccess.Common;
using DataAccess.Entities;
using DataAccess.Models.Features;
using Mapster;
using MapsterMapper;
using Repositories.IRepositories;
using Repositories.Repository;
using Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Services.Service
{
    public class FeatureService : IFeatureService
    {
        private readonly IUnitOfWork _unitOfWork;

        public FeatureService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ApiResponse> GetAllAsync()
        {
            var features = await _unitOfWork.Features.GetAllAsync();
            var result = features.Adapt<List<FeatureDTO>>();

            return new ApiResponse
            {
                IsSuccess = true,
                StatusCode = HttpStatusCode.OK,
                Result = result
            };
        }

        public async Task<ApiResponse> GetPagedAsync(int pageNumber, int pageSize)
        {
            var query = _unitOfWork.Features.AsQueryable();

            var pagedList = await PagedList<Feature>.CreateAsync(query, pageNumber, pageSize);
            var result = pagedList.Adapt<List<FeatureDTO>>();

            return new ApiResponse
            {
                IsSuccess = true,
                StatusCode = HttpStatusCode.OK,
                Result = new
                {
                    pagedList.CurrentPage,
                    pagedList.TotalPages,
                    pagedList.PageSize,
                    pagedList.TotalCount,
                    Items = result
                }
            };
        }

        public async Task<ApiResponse> GetByIdAsync(int id)
        {
            var response = new ApiResponse();
            try
            {
                var feature = await _unitOfWork.Features.GetByIdAsync(id);
                if (feature == null)
                {
                    response.IsSuccess = false;
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.ErrorMessages.Add("Feature Not Found.");
                    return response;
                }

                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.OK;
                response.Result = feature.Adapt<FeatureDTO>();
                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.ErrorMessages.Add(ex.Message);
                return response;
            }
        }

        public async Task<ApiResponse> CreateAsync(FeatureCreateDTO dto)
        {
            var response = new ApiResponse();
            try
            {
                var feature = dto.Adapt<Feature>();
                await _unitOfWork.Features.AddAsync(feature);
                await _unitOfWork.SaveAsync();

                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.Created;
                response.Result = feature.Adapt<FeatureDTO>();
                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.ErrorMessages.Add(ex.Message);
                return response;
            }
        }

        public async Task<ApiResponse> UpdateAsync(FeatureUpdateDTO dto)
        {
            var response = new ApiResponse();
            try
            {
                var feature = await _unitOfWork.Features.GetByIdAsync(dto.Id);
                if (feature == null)
                {
                    response.IsSuccess = false;
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.ErrorMessages.Add("No feature found to update.");
                    return response;
                }

                dto.Adapt(feature);
                _unitOfWork.Features.Update(feature);
                await _unitOfWork.SaveAsync();

                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.OK;
                response.Result = feature.Adapt<FeatureDTO>();
                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.ErrorMessages.Add(ex.Message);
                return response;
            }
        }


        public async Task<ApiResponse> DeleteAsync(int id)
        {
            var response = new ApiResponse();
            try
            {
                var feature = await _unitOfWork.Features.GetByIdAsync(id);
                if (feature == null)
                {
                    response.IsSuccess = false;
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.ErrorMessages.Add("No feature found to delete.");
                    return response;
                }

                _unitOfWork.Features.Delete(feature);
                await _unitOfWork.SaveAsync();

                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.OK;
                response.Result = "Deleted successfully.";
                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.ErrorMessages.Add(ex.Message);
                return response;
            }
        }

    }
}
