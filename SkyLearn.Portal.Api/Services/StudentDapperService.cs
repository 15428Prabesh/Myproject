using Application.Models;
using Core.Constants;
using Core.Helper.FTP;
using Core.Helper.Interfaces;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using NuGet.Packaging.Signing;
using SkyLearn.Portal.Api.Interfaces;
using System.Drawing.Printing;

namespace SkyLearn.Portal.Api.Services
{
    public class StudentDapperService: IStudentDapperService
    {
        private readonly IDapperHelper _dapperHelper;
        private readonly FTP _Ftp;
        public StudentDapperService(IDapperHelper dapperHelper,IOptions<FTP> ftp)
        {
            _dapperHelper = dapperHelper;
            _Ftp = ftp.Value;
        }
        public int Upload(IFormFile file)
        {
            FTPHelper fTPHelper = new FTPHelper();
            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);
                var bytes = stream.ToArray();

                byte[] doc = bytes;
                if (doc != null)
                {
                    string extension = Path.GetExtension(file.FileName);
                    if (!string.IsNullOrEmpty(extension))
                    {
                        string filePath = "student/profile/profile_" + DateTime.Now.Ticks.ToString() +  extension;
                        var response = fTPHelper.UploadFile(_Ftp.Server, _Ftp.UserName, _Ftp.Password, filePath, doc);

                    }
                }
            }
            return 1;
        }
        public async Task<ResponseModel<int>> AddUpdate(string pid, Student student,string userName)
        {
            string profileImage = string.Empty;
            string docFiles=string.Empty;
            ResponseModel<int> responseModel = new ResponseModel<int>();
            var parameters = new DynamicParameters();
            parameters.Add("@Pid", pid);
            parameters.Add("@FirstName", student.FirstName);
            parameters.Add("@MiddleName", student.MiddleName);
            parameters.Add("@LastName", student.LastName);
            parameters.Add("@Gender", student.Gender);
            parameters.Add("@Email", student.Email);
            parameters.Add("@Address", student.Address);
            parameters.Add("@RegistrationNumber", student.RegistrationNumber);
            parameters.Add("@RollNo", student.RollNo);
            parameters.Add("@EnrolledDate", student.EnrolledDate);
            parameters.Add("@Phone", student.Phone);
            parameters.Add("@DOB", student.DOB);
            parameters.Add("@Course", student.Course);
            parameters.Add("@SemesterID", student.SemesterID);
            parameters.Add("@DepartmentID", student.DepartmentID);
            parameters.Add("@FatherName", student.FatherName);
            parameters.Add("@FatherMobile", student.FatherMobile);
            parameters.Add("@MotherName", student.MotherName);
            parameters.Add("@MotherMobile", student.MotherMobile);
            parameters.Add("@ParentEmail", student.ParentEmail);
            parameters.Add("@ParentEmail", student.ParentEmail);

            FTPHelper fTPHelper = new FTPHelper();
            if (student.Profile != null)
            {
                if (student.Profile.ByteString != null)
                {
                    byte[] doc = Convert.FromBase64String(student.Profile.ByteString);
                    if (doc != null)
                    {
                        string extension = Path.GetExtension(student.Profile.FileName);
                        if (!string.IsNullOrEmpty(extension))
                        {
                            string filePath = "student/profile/profile_" + DateTime.Now.Ticks.ToString() + extension;
                            fTPHelper.UploadFile(_Ftp.Server, _Ftp.UserName, _Ftp.Password, filePath, doc);
                            profileImage = filePath;
                        }
                    }
                }
                else
                {
                    if (student.Profile.FileName != null)
                    {
                        profileImage = student.Profile.FileName;
                    }
                }
            }

            if (student.Documents != null && student.Documents.Count > 0)
            {
                var docList = new List<string>();
                foreach (var item in student.Documents)
                {
                    if (item.ByteString != null)
                    {
                        byte[] doc = Convert.FromBase64String(item.ByteString);
                        if (doc != null)
                        {
                            string extension = Path.GetExtension(item.FileName);
                            if (!string.IsNullOrEmpty(extension))
                            {
                                string filePath = "student/document_" + DateTime.Now.Ticks.ToString() + "." + extension;
                                fTPHelper.UploadFile(_Ftp.Server, _Ftp.UserName, _Ftp.Password, filePath, doc);
                                docList.Add(filePath);
                            }
                        }
                    }
                    else
                    {
                        if (item.FileName != null)
                        {
                            docList.Add(item.FileName);
                        }
                    }
                }
                if (docList.Count > 0)
                {
                    docFiles = string.Join(",", docList);
                }
            }
            parameters.Add("@FileDocuments", docFiles);
            parameters.Add("@ProfileImage", profileImage);
            parameters.Add("@UserName", userName);
            var result = await _dapperHelper.Get<int>("usp_Student_AddUpdate", parameters);
            responseModel.Data = result;
            //if (result == -1)
            //{
            //    responseModel.Status = false;
            //    responseModel.Message.Add("Student Email Already exist in system.Please try with different email.");
            //}
            //else if (result == -2)
            //{
            //    responseModel.Status = false;
            //    responseModel.Message.Add("Parent Email Already exist in system.Please try with different email.");
            //}
            //else if (result == 1)
            //{
            //    responseModel.Status = true;
            //    responseModel.Message.Add("Student Added successfully.");
            //}
            //else if (result == 2)
            //{
            //    responseModel.Status = true;
            //    responseModel.Message.Add("Student updated successfully.");
            //}
            //else
            //{
            //    responseModel.Status = true;
            //    responseModel.Message.Add("Error occured in system.Please contact administrator");
            //    responseModel.Data = result;
            //}
            return responseModel;
        }

        public async Task<ResponseModel<List<StudentListDTO>>> GetList(int pageNo,int pageSize,string? searchText, string userName)
        {
            ResponseModel<List<StudentListDTO>> responseModel = new ResponseModel<List<StudentListDTO>>();
            var parameters = new DynamicParameters();
            parameters.Add("@PageNo", pageNo);
            parameters.Add("@PageSize", pageSize);
            parameters.Add("@UserName", userName);
            var result = await _dapperHelper.GetAll<StudentListDTO>("usp_Student_GetList", parameters);
            //if (result != null && result.Count>0)
            //{
            //    responseModel.TotalRows = result.FirstOrDefault().RowTotal;
            //}
            //responseModel.Status = true;
            //responseModel.Message.Add("Success");
            responseModel.Data = result;
            if (result != null && result.Count > 0)
            {
                responseModel.TotalCount = result.FirstOrDefault().RowTotal;
                responseModel.TotalPages = (int)Math.Ceiling((double)responseModel.TotalCount / pageSize);
            }
            return responseModel;
        }

        public async Task<ResponseModel<StudentDTO>> GetByID(string pid, string userName)
        {
            ResponseModel<StudentDTO> responseModel = new ResponseModel<StudentDTO>();
            var parameters = new DynamicParameters();
            parameters.Add("@Pid", pid);
            parameters.Add("@UserName", userName);
            var result = await _dapperHelper.Get<StudentDTO>("usp_Student_GetByID", parameters);
            if (result != null)
            {
                if (!string.IsNullOrEmpty(result.ProfileImage))
                {
                    FileDocumentDto documentDto = new FileDocumentDto();
                    string doc = _Ftp.DocumentHostPath + "/" + result.ProfileImage;
                    documentDto.FileName = result.ProfileImage;
                    documentDto.FilePath = doc;
                    result.Profile = documentDto;
                }

                if (!string.IsNullOrEmpty(result.FileDocuments))
                {
                    List<FileDocumentDto> docs = new List<FileDocumentDto>();
                    var documents = result.FileDocuments?.Split(",").ToList();
                    if (documents != null && documents.Count > 0)
                    {
                        foreach (var item in documents)
                        {
                            if (!string.IsNullOrEmpty(item.Trim()))
                            {
                                FileDocumentDto documentDto = new FileDocumentDto();
                                string doc = _Ftp.DocumentHostPath + "/" + item;
                                documentDto.FileName = item;
                                documentDto.FilePath = doc;
                                docs.Add(documentDto);
                            }
                        }
                    }
                    result.Documents = docs;
                }
            }
            //responseModel.Status = true;
            //responseModel.Message.Add("Success");
            responseModel.Data = result;
            return responseModel;
        }

        public async Task<ResponseModel<int>> ActivateDeactivate(string pid, bool isActive, string userName)
        {
            ResponseModel<int> responseModel = new ResponseModel<int>();
            var parameters = new DynamicParameters();
            parameters.Add("@Pid", pid);
            parameters.Add("@IsActive", isActive);
            parameters.Add("@UserName", userName);
            var result = await _dapperHelper.Get<int>("usp_Student_ActivateDeactivate", parameters);
            //responseModel.Status = true;
            //responseModel.Message.Add("Success");
            responseModel.Data = result;
            return responseModel;
        }

        public async Task<int> UpdateStudentUser(string pid, string userName)
        {            
            var parameters = new DynamicParameters();
            parameters.Add("@Pid", pid);
            parameters.Add("@UserName", userName);
            var result = await _dapperHelper.Get<int>("usp_Student_GetByID", parameters);
            return result;
        }

        public async Task<int> UpdateParentUser(string pid, string userName)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Pid", pid);
            parameters.Add("@UserName", userName);
            var result = await _dapperHelper.Get<int>("usp_Student_GetByID", parameters);
            return result;
        }
    }
}
