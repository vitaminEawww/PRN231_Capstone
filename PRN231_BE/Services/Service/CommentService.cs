using DataAccess.Common;
using DataAccess.Entities;
using DataAccess.Models.Comments;
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
    public class CommentService : ICommentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CommentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;   
        }
        public async Task<ApiResponse> CreateAsync(CommentCreateDTO dto)
        {
            var response = new ApiResponse();
            try
            {
                var comment = dto.Adapt<Comment>();
                comment.CreatedDate = DateTime.UtcNow;
                await _unitOfWork.Comments.AddAsync(comment);
                await _unitOfWork.SaveAsync();

                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.Created;
                response.Result = comment.Adapt<CommentDTO>();
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
                var comment = await _unitOfWork.Comments.GetByIdAsync(id);
                if (comment == null)
                {
                    response.IsSuccess = false;
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.ErrorMessages.Add("Comment not found to deleted");
                    return response;
                }

                comment.IsDeleted = false;
                _unitOfWork.Comments.Update(comment);
                await _unitOfWork.SaveAsync();

                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.OK;
                response.Result = "Deleted successful";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.ErrorMessages.Add(ex.Message);
            }
            return response;
        }

        public async Task<ApiResponse> GetAllAsync()
        {
            var response = new ApiResponse();
            try
            {
                var comments = await _unitOfWork.Comments.GetAllAsync();
                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.OK;
                response.Result = comments.Adapt<List<CommentDTO>>();
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
                var comment = await _unitOfWork.Comments.GetByIdAsync(id);
                if (comment == null)
                {
                    response.IsSuccess = false;
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.ErrorMessages.Add("Comment not found!");
                    return response;
                }

                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.OK;
                response.Result = comment.Adapt<CommentDTO>();
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.ErrorMessages.Add(ex.Message);
            }
            return response;
        }

        public async Task<ApiResponse> GetByPostIdAsync(int postId)
        {
            var response = new ApiResponse();
            try
            {
                var comments = _unitOfWork.Comments.AsQueryable()
                    .Where(c => c.PostId == postId)
                    .ToList();

                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.OK;
                response.Result = comments.Adapt<List<CommentDTO>>();
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.ErrorMessages.Add(ex.Message);
            }
            return response;
        }

        public async Task<ApiResponse> UpdateAsync(CommentUpdateDTO dto)
        {
            var response = new ApiResponse();
            try
            {
                var comment = await _unitOfWork.Comments.GetByIdAsync(dto.Id);
                if (comment == null)
                {
                    response.IsSuccess = false;
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.ErrorMessages.Add("Comment not found to update");
                    return response;
                }

                dto.Adapt(comment);
                _unitOfWork.Comments.Update(comment);
                await _unitOfWork.SaveAsync();

                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.OK;
                response.Result = comment.Adapt<CommentDTO>();
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
