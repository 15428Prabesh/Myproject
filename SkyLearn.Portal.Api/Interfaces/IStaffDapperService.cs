using Application.Models;

namespace SkyLearn.Portal.Api.Interfaces
{
    public interface IStaffDapperService
    {
        Task<ResponseModel<int>> AddUpdate(string pid, Staff staff, string userName);
        Task<ResponseModel<List<StaffListDTO>>> GetList(int pageNo, int pageSize, string? searchText, string userName);
        Task<ResponseModel<StaffDTO>> GetByID(string pid, string userName);
        Task<ResponseModel<int>> ActivateDeactivate(string pid, bool isActive, string userName);
        Task<int> UpdateStaffUser(string pid, string userName);
    }
}
