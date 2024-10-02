using Application.Models;

namespace SkyLearn.Portal.Api.Interfaces
{
    public interface IStudentDapperService
    {
        Task<ResponseModel<int>> AddUpdate(string pid, Student student, string userName);
        Task<ResponseModel<List<StudentListDTO>>> GetList(int pageNo, int pageSize, string? searchText, string userName);
        Task<ResponseModel<StudentDTO>> GetByID(string pid, string userName);
        Task<ResponseModel<int>> ActivateDeactivate(string pid, bool isActive, string userName);
        int Upload(IFormFile file);
    }
}
