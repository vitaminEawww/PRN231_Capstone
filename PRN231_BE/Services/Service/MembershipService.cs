using DataAccess.Common;
using DataAccess.Entities;
using DataAccess.Models.MemberShips;
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
    public class MembershipService : IMembershipService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MembershipService( IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ApiResponse> CreateAsync(MembershipCreateDTO dto)
        {
            var response = new ApiResponse();
            try 
            {
                var entity = dto.Adapt<Membership>();
                await _unitOfWork.MemberShips.AddAsync(entity);
                await _unitOfWork.SaveAsync();
                response.IsSuccess = true;
                response.StatusCode = System.Net.HttpStatusCode.Created;
                response.Result = entity.Adapt<MemberShipsDTO>();
            }
            catch(Exception ex)
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
                var entity = await _unitOfWork.MemberShips.GetByIdAsync(id);
                if (entity == null)
                {
                    response.IsSuccess = false;
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.ErrorMessages.Add("Membership not found.");
                    return response;
                }
                _unitOfWork.MemberShips.Delete(entity);
                await _unitOfWork.SaveAsync();
                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.NoContent; // 204 No Content
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.ErrorMessages.Add(ex.Message);
            }
            return response;
        }

        public  async Task<ApiResponse> GetAllAsync()
        {
            var response = new ApiResponse();
            try
            {
                var list = await _unitOfWork.MemberShips.GetAllAsync();
                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.OK;
                response.Result = list.Adapt<List<MemberShipsDTO>>();
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.ErrorMessages.Add(ex.Message);
            }
            return response;
        }

        public async Task<ApiResponse> GetByIdAsync(int id)
        {
            var response = new ApiResponse();
            try
            {
                var entity = await _unitOfWork.MemberShips.GetByIdAsync(id);
                if (entity == null)
                {
                    response.IsSuccess = false;
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.ErrorMessages.Add("Membership not found.");
                    return response;
                }

                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.OK;
                response.Result = entity.Adapt<MemberShipsDTO>();
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.ErrorMessages.Add(ex.Message);
            }
            return response;
        }

        public async Task<ApiResponse> GetPagedAsync(int pageNumber, int pageSize)
        {
            var response = new ApiResponse();
            try
            {
                var query = _unitOfWork.MemberShips.AsQueryable();
                var pagedList = await PagedList<Membership>.CreateAsync(query, pageNumber, pageSize);

                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.OK;
                response.Result = new
                {
                    pagedList.CurrentPage,
                    pagedList.TotalPages,
                    pagedList.PageSize,
                    pagedList.TotalCount,
                    Items = pagedList.Adapt<List<MemberShipsDTO>>()
                };
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.ErrorMessages.Add(ex.Message);
            }
            return response;
        }

        public async Task<ApiResponse> UpdateAsync(MemberShipUpdateDTO dto)
        {
            var response = new ApiResponse();
            try
            {
                var entity = await _unitOfWork.MemberShips.GetByIdAsync(dto.PlanId);
                if (entity == null)
                {
                    response.IsSuccess = false;
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.ErrorMessages.Add("No membership found to update.");
                    return response;
                }

                dto.Adapt(entity);
                _unitOfWork.MemberShips.Update(entity);
                await _unitOfWork.SaveAsync();

                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.OK;
                response.Result = entity.Adapt<MemberShipsDTO>();
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
