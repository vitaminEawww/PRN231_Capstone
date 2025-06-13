using DataAccess.Common;
using DataAccess.Models.SmokingRecords;

namespace Services.IServices
{
    public interface ISmokingRecordService
    {
        Task<ApiResponse> CreateSmokingRecordAsync(SmokingRecordCreateDTO dto);
        Task<ApiResponse> GetCurrentSmokingRecordAsync(int customerId);
        Task<ApiResponse> GetSmokingHistoryAsync(int customerId, int pageNumber = 1, int pageSize = 10);
        Task<ApiResponse> UpdateSmokingRecordAsync(int id, SmokingRecordUpdateDTO dto);
        Task<ApiResponse> DeleteSmokingRecordAsync(int id);
        Task<ApiResponse> GetSmokingRecordByIdAsync(int id);
    }
}