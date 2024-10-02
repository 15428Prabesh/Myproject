using Application.Models;

namespace SkyLearn.Portal.Api.Interfaces
{
    public interface IAssignmentEnrollService
    {
        Task<ResponseModel<int>> AssignStudent(string assignmentID, List<string> studentList, bool isAll, string userName);
        Task<ResponseModel<List<StudentAssignmentListDto>>> GetAdminAssignStudentList(int pageNo, int pageSize, string? assignmentID, string? status, string userName);
        Task<ResponseModel<List<StudentAssignmentListDto>>> GetAllParentStudentAssignment(int pageSize, int pageNumber, string? searchText, string? status, string userName);
        Task<ResponseModel<StudentAssignmentDto>> GetParentStudentAssignementDetail(string id, string userName);
        Task<ResponseModel<List<StudentAssignmentLogListDto>>> GetParentSTudentAssignementLogList(int pageSize, int pageNumber,string pid, string userName);
        Task<ResponseModel<StudentAssignmentSummaryDTO>> GetParentStudentAssignementSummary(string userName);
        Task<ResponseModel<StudentAssignmentSummaryDTO>> GetStudentAssignementSummary(string userName);
        Task<ResponseModel<StudentAssignmentSummaryDTO>> GetStaffStudentAssignementSummary(string userName);
        Task<ResponseModel<StudentAssignmentSummaryDTO>> GetAdminStudentAssignementSummary(string userName);
        Task<ResponseModel<int>> ApproveAssignment(string id, string remarks, string userName);
        Task<ResponseModel<int>> RejectAssignment(string id, string remarks, string userName);
        Task<ResponseModel<List<StudentAssignmentListDto>>> GetAllStaffAssignment( int pageSize, int pageNumber, string? searchText, string? status, string userName);
        Task<ResponseModel<List<StudentAssignmentListDto>>> GetStaffStudentAssignementList(string id, string? searchText, string? status, int pageSize , int pageNumber,string userName);
        Task<ResponseModel<StudentAssignmentDto>> GetAssignementDetail(string id, string userName);        
        Task<ResponseModel<List<StudentAssignmentLogListDto>>> GetStaffStudentAssignementLogList(string assignemtID, string id, int pageSize, int pageNumber, string userName);
        Task<ResponseModel<int>> SubmitAssignment(string id, SubmissionInfo submission, string userName);
        Task<ResponseModel<List<StudentAssignmentListDto>>> GetAllStudentAssignment(int pageSize, int pageNumber , string? searchText,string? status, string userName);
        Task<ResponseModel<List<StudentAssignmentListDto>>> GetAllAdminStudentAssignment(int pageSize, int pageNumber, string? searchText, string? status, string userName);
       
        Task<ResponseModel<List<StudentAssignmentLogListDto>>> GetSTudentAssignementLogList(string pid, int pageSize, int pageNumber, string userName);
        Task<ResponseModel<List<DropDown>>> GetStatusList();
    }
}
