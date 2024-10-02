using Application.Helpers;
using Application.Models;
using Core.Constants;
using Core.Helper.FTP;
using Core.Helper.Interfaces;
using Dapper;
using Microsoft.Extensions.Options;
using SkyLearn.Portal.Api.Interfaces;
using System.Drawing.Printing;

namespace SkyLearn.Portal.Api.Services
{
    public class AssignmentEnrollService: IAssignmentEnrollService
    {
        private readonly IDapperHelper _dapperHelper;
        private readonly FTP _Ftp;
        private readonly IEmailSendService _emailSendService;
        public AssignmentEnrollService(IDapperHelper dapperHelper, IOptions<FTP> ftp, IEmailSendService emailSendService) {
            _dapperHelper=dapperHelper;
            _Ftp = ftp.Value;
            _emailSendService=emailSendService;
        }

        public async Task<ResponseModel<int>> ApproveAssignment(string id, string remarks, string userName)
        {
            ResponseModel<int> responseModel = new ResponseModel<int>();
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Pid", id);
            parameters.Add("@Remarks", remarks);
            parameters.Add("@Username", userName);
            var result = await _dapperHelper.Get<int>("usp_StudentAssignment_Approve", parameters);
            responseModel.Data = result;
            //if (result > 0)
            //{
            //    responseModel.Status = true;
            //    responseModel.Message.Add("Assignment approve successfully.");
            //}
            //else if (result == -1)
            //{
            //    responseModel.Status = false;
            //    responseModel.Message.Add("Assignment is in proccess.You cannot approve this assignemnt.");
            //}
            //else
            //{
            //    responseModel.Status = false;
            //    responseModel.Message.Add("Error occured in system.Please contact administrator.");

            //}
            return responseModel;
        }

        public async Task<ResponseModel<int>> AssignStudent(string assignmentID, List<string> studentList, bool isAll, string userName)
        {
            ResponseModel<int> responseModel = new ResponseModel<int>();
            foreach (var item in studentList)
            {
                string pid= AppHelper.GeneratePid(Constant.PREFIX_STUDENT);
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@Pid", pid);
                parameters.Add("@AssignmentID", assignmentID);
                parameters.Add("@StudentID", item);
                parameters.Add("@Username", userName);
                int result = await _dapperHelper.Insert<int>("usp_StudentAssignment_AssignStudent", parameters);
                //if (result > 0)
                //{
                //    responseModel.Message.Add("Student with pid " + item + " assigned successfully.");
                //}
                //else if (result > 0)
                //{
                //    responseModel.Message.Add("Student with pid " + item + " already exists.");

                //}
                //else 
                //{
                //    responseModel.Message.Add("Student with pid " + item + " error occureed.");
                //}
            }
           // responseModel.Status = true;
            responseModel.Data = 1;            
            return responseModel;
        }

        public async Task<ResponseModel<List<StudentAssignmentListDto>>> GetAllParentStudentAssignment(int pageSize, int pageNumber, string? searchText, string? status, string userName)
        {
            ResponseModel<List<StudentAssignmentListDto>> responseModel = new ResponseModel<List<StudentAssignmentListDto>>();
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@PageSize", pageSize);
            parameters.Add("@PageNumber", pageNumber);
            parameters.Add("@SearchText", searchText);
            parameters.Add("@Status", status);
            parameters.Add("@Username", userName);
            var result = await _dapperHelper.GetAll<StudentAssignmentListDto>("usp_StudentAssignment_GetAllParentStudentAssignment", parameters);
           // responseModel.Status = true;
            responseModel.Data = result;
            if (result != null && result.Count>0)
            {
                responseModel.TotalCount = result.FirstOrDefault().RowTotal;
                responseModel.TotalPages = (int)Math.Ceiling((double)responseModel.TotalCount / pageSize);
            }
            // responseModel.Message.Add("success");
            return responseModel;
        }

        public async Task<ResponseModel<List<StudentAssignmentListDto>>> GetAllStaffAssignment(int pageSize, int pageNumber, string? searchText,string?status, string userName)
        {
            ResponseModel<List<StudentAssignmentListDto>> responseModel = new ResponseModel<List<StudentAssignmentListDto>>();
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@PageSize", pageSize);
            parameters.Add("@PageNumber", pageNumber);
            parameters.Add("@SearchText", searchText);
            parameters.Add("@Status", status);
            parameters.Add("@Username", userName);
            var result = await _dapperHelper.GetAll<StudentAssignmentListDto>("usp_StudentAssignment_GetAllStaffAssignment", parameters);
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

        public async Task<ResponseModel<List<StudentAssignmentListDto>>> GetAllStudentAssignment(int pageSize, int pageNumber, string? searchText,string? status, string userName)
        {
            ResponseModel<List<StudentAssignmentListDto>> responseModel = new ResponseModel<List<StudentAssignmentListDto>>();
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@PageSize", pageSize);
            parameters.Add("@PageNumber", pageNumber);
            parameters.Add("@SearchText", searchText);
            parameters.Add("@Status", status);
            parameters.Add("@Username", userName);
            var result = await _dapperHelper.GetAll<StudentAssignmentListDto>("usp_StudentAssignment_GetAllStudentAssignment", parameters);
          //  responseModel.Status = true;
            responseModel.Data = result;
            if (result != null && result.Count > 0)
            {
                responseModel.TotalCount = result.FirstOrDefault().RowTotal;
                responseModel.TotalPages = (int)Math.Ceiling((double)responseModel.TotalCount / pageSize);
            }
            //   responseModel.Message.Add("success");
            return responseModel;
        }

        public async Task<ResponseModel<List<StudentAssignmentListDto>>> GetAllAdminStudentAssignment(int pageSize, int pageNumber, string? searchText,string? status, string userName)
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
       

        public async Task<ResponseModel<List<StudentAssignmentListDto>>> GetAdminAssignStudentList(int pageNo, int pageSize, string? assignmentID, string? status, string userName)
        {
            ResponseModel<List<StudentAssignmentListDto>> responseModel = new ResponseModel<List<StudentAssignmentListDto>>();
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@PageNo", pageNo);
            parameters.Add("@PageSize", pageSize);
            parameters.Add("@AssignmentID", assignmentID);
            parameters.Add("@Status", status);
            parameters.Add("@Username", userName);
            var result = await _dapperHelper.GetAll<StudentAssignmentListDto>("usp_StudentAssignment_GetAdminAssignStudentList", parameters);
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

        public async Task<ResponseModel<StudentAssignmentDto>> GetParentStudentAssignementDetail(string id, string userName)
        {
            ResponseModel<StudentAssignmentDto> responseModel = new ResponseModel<StudentAssignmentDto>();
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Pid", id);           
            parameters.Add("@Username", userName);
            var result = await _dapperHelper.Get<StudentAssignmentDto>("usp_StudentAssignment_GetParentStudentAssignementDetail", parameters);
          //  responseModel.Status = true;
            responseModel.Data = result;
           // responseModel.Message.Add("success");
            return responseModel;
        }

        public async Task<ResponseModel<List<StudentAssignmentLogListDto>>> GetParentSTudentAssignementLogList(int pageSize, int pageNumber,string pid, string userName)
        {
            ResponseModel<List<StudentAssignmentLogListDto>> responseModel = new ResponseModel<List<StudentAssignmentLogListDto>>();
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Pid", pid);
            parameters.Add("@PageSize", pageSize);
            parameters.Add("@PageNumber", pageNumber);
            parameters.Add("@Username", userName);
            var result = await _dapperHelper.GetAll<StudentAssignmentLogListDto>("usp_StudentAssignment_GetParentSTudentAssignementLogList", parameters);
            result = ReturnWithFileINLog(result);
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

        public async Task<ResponseModel<StudentAssignmentSummaryDTO>> GetParentStudentAssignementSummary(string userName)
        {
            ResponseModel<StudentAssignmentSummaryDTO> responseModel = new ResponseModel<StudentAssignmentSummaryDTO>();
            DynamicParameters parameters = new DynamicParameters();           
            parameters.Add("@Username", userName);
            var result = await _dapperHelper.Get<StudentAssignmentSummaryDTO>("usp_StudentAssignment_GetParentStudentAssignementSummary", parameters);
          //  responseModel.Status = true;
            responseModel.Data = result;
          //  responseModel.Message.Add("success");
            return responseModel;
        }

        public async Task<ResponseModel<StudentAssignmentSummaryDTO>> GetStaffStudentAssignementSummary(string userName)
        {
            ResponseModel<StudentAssignmentSummaryDTO> responseModel = new ResponseModel<StudentAssignmentSummaryDTO>();
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Username", userName);
            var result = await _dapperHelper.Get<StudentAssignmentSummaryDTO>("usp_StudentAssignment_GetStaffStudentAssignementSummary", parameters);
           // responseModel.Status = true;
            responseModel.Data = result;
          //  responseModel.Message.Add("success");
            return responseModel;
        }

        public async Task<ResponseModel<StudentAssignmentSummaryDTO>> GetAdminStudentAssignementSummary(string userName)
        {
            ResponseModel<StudentAssignmentSummaryDTO> responseModel = new ResponseModel<StudentAssignmentSummaryDTO>();
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Username", userName);
            var result = await _dapperHelper.Get<StudentAssignmentSummaryDTO>("usp_StudentAssignment_GetAdminStudentAssignementSummary", parameters);
          //  responseModel.Status = true;
            responseModel.Data = result;
          //  responseModel.Message.Add("success");
            return responseModel;
        }

        public async Task<ResponseModel<StudentAssignmentSummaryDTO>> GetStudentAssignementSummary(string userName)
        {
            ResponseModel<StudentAssignmentSummaryDTO> responseModel = new ResponseModel<StudentAssignmentSummaryDTO>();
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Username", userName);
            var result = await _dapperHelper.Get<StudentAssignmentSummaryDTO>("usp_StudentAssignment_GetStudentAssignementSummary", parameters);
          //  responseModel.Status = true;
            responseModel.Data = result;
          //  responseModel.Message.Add("success");
            return responseModel;
        }

        public async Task<ResponseModel<List<StudentAssignmentListDto>>> GetStaffStudentAssignementList(string id, string? searchText, string? status, int pageSize, int pageNumber, string userName)
        {
            ResponseModel<List<StudentAssignmentListDto>> responseModel = new ResponseModel<List<StudentAssignmentListDto>>();
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Pid", id);
            parameters.Add("@PageSize", pageSize);
            parameters.Add("@PageNumber", pageNumber);
            parameters.Add("@SearchText", searchText);
            parameters.Add("@Status", status);
            parameters.Add("@Username", userName);
            var result = await _dapperHelper.GetAll<StudentAssignmentListDto>("usp_StudentAssignment_GetStaffStudentAssignementList", parameters);
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

        public async Task<ResponseModel<List<StudentAssignmentLogListDto>>> GetStaffStudentAssignementLogList(string assignmentID, string id, int pageSize, int pageNumber, string userName)
        {
            ResponseModel<List<StudentAssignmentLogListDto>> responseModel = new ResponseModel<List<StudentAssignmentLogListDto>>();
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@AssignmentID", assignmentID);
            parameters.Add("@Pid", id);
            parameters.Add("@PageSize", pageSize);
            parameters.Add("@PageNumber", pageNumber);
            parameters.Add("@Username", userName);
            var  result = await _dapperHelper.GetAll<StudentAssignmentLogListDto>("usp_StudentAssignment_GetStaffStudentAssignementLogList", parameters);
            result = ReturnWithFileINLog(result);
           // responseModel.Status = true;
            responseModel.Data = result;
            if (result != null && result.Count > 0)
            {
                responseModel.TotalCount = result.FirstOrDefault().RowTotal;
                responseModel.TotalPages = (int)Math.Ceiling((double)responseModel.TotalCount / pageSize);
            }
            // responseModel.Message.Add("success");
            return responseModel;
        }
        

        private List<StudentAssignmentLogListDto> ReturnWithFileINLog(List<StudentAssignmentLogListDto> result)
        {
            if (result != null && result.Count > 0)
            {
                foreach (var item in result)
                {
                    List<FileDocumentDto> fileDto = new List<FileDocumentDto>();
                    if (item.FileDocument != null)
                    {

                        var images = item.FileDocument.Split(",");
                        foreach (var image in images)
                        {
                            FileDocumentDto fileDocument = new FileDocumentDto();
                            fileDocument.FileName = image;
                            fileDocument.FilePath = _Ftp.DocumentHostPath + "/" + image;

                            fileDto.Add(fileDocument);

                        }
                    }
                    item.FileDocuments= fileDto;
                }
            }
            return result;
        }        

        public async Task<ResponseModel<List<StudentAssignmentLogListDto>>> GetSTudentAssignementLogList(string pid, int pageSize, int pageNumber, string userName)
        {
            ResponseModel<List<StudentAssignmentLogListDto>> responseModel = new ResponseModel<List<StudentAssignmentLogListDto>>();
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Pid", pid);
            parameters.Add("@PageSize", pageSize);
            parameters.Add("@PageNumber", pageNumber);
            parameters.Add("@Username", userName);
            var result = await _dapperHelper.GetAll<StudentAssignmentLogListDto>("usp_StudentAssignment_GetSTudentAssignementLogList", parameters);
            result = ReturnWithFileINLog(result);
           // responseModel.Status = true;
            responseModel.Data = result;
            if (result != null && result.Count > 0)
            {
                responseModel.TotalCount = result.FirstOrDefault().RowTotal;
                responseModel.TotalPages = (int)Math.Ceiling((double)responseModel.TotalCount / pageSize);
            }
            // responseModel.Message.Add("success");
            return responseModel;
        }

        public async Task<ResponseModel<List<DropDown>>> GetStatusList()
        {
            ResponseModel<List<DropDown>> responseModel = new ResponseModel<List<DropDown>>();
            DynamicParameters parameters = new DynamicParameters();           
            var result = await _dapperHelper.GetAll<DropDown>("usp_StudentAssignment_GetStatus", parameters);           
          //  responseModel.Status = true;
            responseModel.Data = result;
            if (result != null && result.Count > 0)
            {
                responseModel.TotalCount = result.Count;
                responseModel.TotalPages = (int)Math.Ceiling((double)responseModel.TotalCount / result.Count);
            }
            // responseModel.Message.Add("success");
            return responseModel;
        }

        public async Task<ResponseModel<int>> RejectAssignment(string id, string remarks, string userName)
        {
            ResponseModel<int> responseModel = new ResponseModel<int>();
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Pid", id);
            parameters.Add("@Remarks", remarks);
            parameters.Add("@Username", userName);
            int result = await _dapperHelper.Insert<int>("usp_StudentAssignment_RejectAssignment", parameters);
            responseModel.Data = result;
            //if (result > 0)
            //{
            //    responseModel.Status = true;
            //    responseModel.Message.Add("Assignment rejected successfully.");
            //}
            //else if (result == -1)
            //{
            //    responseModel.Status = false;
            //    responseModel.Message.Add("Assignment is in proccess.You cannot reject this assignemnt.");
            //}
            //else
            //{
            //    responseModel.Status = false;
            //    responseModel.Message.Add("Error occured in system.Please contact administrator.");

            //}
            return responseModel;
        }

        public async Task<ResponseModel<int>> SubmitAssignment(string id, SubmissionInfo submission, string userName)
        {
            ResponseModel<int> responseModel = new ResponseModel<int>();
            DynamicParameters parameters = new DynamicParameters();

            string docFiles = string.Empty;
            if (submission.FileDocuemnt != null && submission.FileDocuemnt.Count > 0)
            {
                var docList = new List<string>();
                FTPHelper fTPHelper = new FTPHelper();
                string ftpServer = string.Empty;
                string ftpUserName = string.Empty;
                string ftpPassword = string.Empty;
                foreach (var item in submission.FileDocuemnt)
                {
                    if(item !=null && item.ByteString !=null)
                            {
                        byte[] doc = Convert.FromBase64String(item.ByteString);
                        if (doc != null)
                        {
                            string extension = Path.GetExtension(item.FileName);
                            if (!string.IsNullOrEmpty(extension))
                            {
                                string filePath = "studentassignment/document/document_" + DateTime.Now.Ticks.ToString() + extension;
                                fTPHelper.UploadFile(ftpServer, ftpUserName, ftpPassword, filePath, doc);
                                docList.Add(filePath);
                            }
                        }
                    }
                }
                if (docList.Count > 0)
                {
                    docFiles = string.Join(",", docList);
                }

            }

            parameters.Add("@Pid", id);
            parameters.Add("@Remarks", submission.Remarks);
            parameters.Add("@FileDocument", docFiles);
            parameters.Add("@Username", userName);
            int result = await _dapperHelper.Insert<int>("usp_StudentAssignment_SubmitAssignment", parameters);
            responseModel.Data = result;
            //if (result > 0)
            //{
            //    responseModel.Status = true;
            //    responseModel.Message.Add("Assignment submitted successfully.");
            //}
            //else if (result == -1)
            //{
            //    responseModel.Status = false;
            //    responseModel.Message.Add("Assignment is in proccess.You cannot submitted this assignemnt.");
            //}
            //else
            //{
            //    responseModel.Status = false;
            //    responseModel.Message.Add("Error occured in system.Please contact administrator.");

            //}
            return responseModel;
        }

        private async Task SendStudentAssignmentStatusChangeEmail(string status, string email,string pid, string origin)
        {
            string subject = string.Empty;
            if (status.ToLower() == "pending")
            {
                subject = "New Assignment";
            }
            else
            {
                subject = "Assignment " + status;
            }
            string message;
            if (!string.IsNullOrEmpty(origin))
                message = $@"<p>You have got new assignemnt. Please click link to visti there.<a href=""{origin}/account/forgot-password"">Click Here</a></p>";
            else
                message = "<p>You have got new assignemnt. You can check it via the <code>/accounts/forgot-password</code> api route.</p>";

            await _emailSendService.Send(
                to: email,
                subject: subject,
                html: $@"
                        {message}"
            );
        }

        private async Task SendStaffAssignmentEmail(string email, string pid, string origin)
        {
            string subject = "New Assignment";
            
            string message;
            if (!string.IsNullOrEmpty(origin))
                message = $@"<p>You have got new assignemnt. Please click link to visti there.<a href=""{origin}/account/forgot-password"">Click Here</a></p>";
            else
                message = "<p>You have got new assignemnt. You can check it via the <code>/accounts/forgot-password</code> api route.</p>";

            await _emailSendService.Send(
                to: email,
                subject: subject,
                html: $@"
                        {message}"
            );
        }

    }
}
