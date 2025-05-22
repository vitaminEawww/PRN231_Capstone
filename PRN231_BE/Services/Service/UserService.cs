﻿using DataAccess.Common;
using DataAccess.Entities;
using DataAccess.Models.Users;
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
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly JwtTokenService _jwtTokenService;
        public UserService(IUnitOfWork unitOfWork, JwtTokenService jwtTokenService)
        {
            _unitOfWork = unitOfWork;
            _jwtTokenService = jwtTokenService;
        }
        public async Task<ApiResponse> CreateUserAsync(UserCreateDTO userDto)
        {
            var response = new ApiResponse();
            try
            {
                var existingUser = (await _unitOfWork.Users
                    .GetAllAsync())
                    .FirstOrDefault(u => u.Email == userDto.Email || u.Username == userDto.Username);

                if (existingUser != null)
                {
                    response.IsSuccess = false;
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.ErrorMessages.Add("Email hoặc Username đã tồn tại.");
                    return response;
                }

                var user = userDto.Adapt<User>();
                user.PasswordHash = HashPassword(userDto.Password);
                user.JoinDate = DateTime.Now;
                user.Status = true;

                await _unitOfWork.Users.AddAsync(user);
                await _unitOfWork.SaveAsync();

                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.Created;
                response.Result = user; 

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

        public async  Task<ApiResponse> DeleteUserAsync(int userId)
        {
            var response = new ApiResponse();
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    response.IsSuccess = false;
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.ErrorMessages.Add("User không tồn tại.");
                    return response;
                }

                // Soft delete: chỉ đổi Status thành false
                user.Status = false;

                _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveAsync();

                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.OK;
                response.Result = "User đã được vô hiệu hóa.";

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

        public async Task<ApiResponse> GetAllUserAsync(int pageNumber, int pageSize)
        {
            var response = new ApiResponse();
            try
            {
                var query = _unitOfWork.Users.AsQueryable();

                var pagedUsers = await PagedList<User>.CreateAsync(query, pageNumber, pageSize);

                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.OK;
                response.Result = pagedUsers;

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

        public async Task<ApiResponse> GetUserByIdAsync(int userId)
        {
            var response = new ApiResponse();
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    response.IsSuccess = false;
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.ErrorMessages.Add("User không tồn tại.");
                    return response;
                }

                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.OK;
                response.Result = user;
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

        public async Task<ApiResponse> LoginAsync(string email, string password)
        {
            var response = new ApiResponse();
            try
            {
                var user = (await _unitOfWork.Users.GetAllAsync())
                            .FirstOrDefault(u => u.Email == email);

                if (user == null)
                {
                    response.IsSuccess = false;
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.ErrorMessages.Add("Email không tồn tại.");
                    return response;
                }

                if (!user.Status)
                {
                    response.IsSuccess = false;
                    response.StatusCode = HttpStatusCode.Forbidden;
                    response.ErrorMessages.Add("Tài khoản đã bị khóa.");
                    return response;
                }

                bool passwordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
                if (!passwordValid)
                {
                    response.IsSuccess = false;
                    response.StatusCode = HttpStatusCode.Unauthorized;
                    response.ErrorMessages.Add("Mật khẩu không đúng.");
                    return response;
                }

                var token = _jwtTokenService.GenerateToken(user);

                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.OK;
                response.Result = new { Token = token, User = user };

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

        public async Task<ApiResponse> UpdateUserAsync(UserUpdateDTO userDto)
        {
            var response = new ApiResponse();
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userDto.UserID);
                if (user == null)
                {
                    response.IsSuccess = false;
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.ErrorMessages.Add("User không tồn tại.");
                    return response;
                }

                // Cập nhật các trường được phép
                user.FullName = userDto.FullName;
                user.Username = userDto.Username;

                _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveAsync();

                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.OK;
                response.Result = user;

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

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}
