using Application.Models;
using Core.Helper.Interfaces;
using Dapper;
using Microsoft.Extensions.Options;

namespace SkyLearn.Portal.Api.Services
{
    public class AssignmentDapperService
    {
        private readonly IDapperHelper _dapperHelper;
        public AssignmentDapperService(IDapperHelper dapperHelper)
        {
            _dapperHelper = dapperHelper;
        }

        public async Task<ResponseModel<List<StudentAssignmentListDto>>> GetAllAdminStudentAssignment(int pageSize, int pageNumber, string? searchText, string? status, string userName)
        {
            ResponseModel<List<StudentAssignmentListDto>> responseModel = new ResponseModel<List<StudentAssignmentListDto>>();
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@PageSize", pageSize);
            parameters.Add("@PageNumber", pageNumber);
            parameters.Add("@SearchText", searchText);
            parameters.Add("@Status", status);
            parameters.Add("@Username", userName);
            var result = await _dapperHelper.GetAll<StudentAssignmentListDto>("usp_StudentAssignment_GetAllAdminStudentAssignment", parameters);
            //  responseModel.Status = true;
            responseModel.Data = result;
            if (result != null && result.Count > 0)
            {
                responseModel.TotalCount = result.FirstOrDefault().RowTotal;
                responseModel.TotalPages = (int)Math.Ceiling((double)responseModel.TotalCount / pageSize);
            }
            //  responseModel.Message.Add("success");
            return responseModel;
        }

        public async Task<ResponseModel<StudentAssignmentDto>> GetAssignementDetail(string id, string userName)
        {
            ResponseModel<StudentAssignmentDto> responseModel = new ResponseModel<StudentAssignmentDto>();
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Pid", id);
            parameters.Add("@Username", userName);
            var result = await _dapperHelper.Get<StudentAssignmentDto>("usp_StudentAssignment_GetAssignementDetail", parameters);
            //  responseModel.Status = true;
            responseModel.Data = result;
            //  responseModel.Message.Add("success");
            return responseModel;
        }
    }
}
