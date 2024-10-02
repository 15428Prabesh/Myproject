using Application.Models;
using Core.Constants;
using Core.Helper.FTP;
using Core.Helper.Interfaces;
using Dapper;
using Microsoft.CodeAnalysis.Options;
using NuGet.Protocol;
using SkyLearn.Portal.Api.Interfaces;

namespace SkyLearn.Portal.Api.Services
{
    public class StaffDapperService : IStaffDapperService
    {
        private readonly IDapperHelper _dapperHelper;
        private readonly FTP _Ftp;
        public StaffDapperService(IDapperHelper dapperHelper, Microsoft.Extensions.Options.IOptions<FTP> ftp)
        {
            _dapperHelper = dapperHelper;
            _Ftp = ftp.Value;
        }
        public async Task<ResponseModel<int>> AddUpdate(string pid, Staff staff, string userName)
        {
            string profileImage = string.Empty;
            string fileDocument = string.Empty;
            ResponseModel<int> responseModel = new ResponseModel<int>();
            var parameters = new DynamicParameters();
            parameters.Add("@Pid", pid);
            parameters.Add("@FirstName", staff.FirstName);
            parameters.Add("@MiddleName", staff.MiddleName);
            parameters.Add("@LastName", staff.LastName);
            parameters.Add("@Gender", staff.Gender);
            parameters.Add("@Email", staff.Email);
            parameters.Add("@Address", staff.Address);
            parameters.Add("@Phone", staff.Phone);
            parameters.Add("@DOB", staff.DOB);
            parameters.Add("@JoiningDate", staff.JoiningDate);
            parameters.Add("@Postion", staff.Position);
            parameters.Add("@DepartmentID", staff.DepartmentId);
            parameters.Add("@Postion", staff.Position);
            parameters.Add("@UserName", userName);


            FTPHelper fTPHelper = new FTPHelper();
            if (staff.Profile != null)
            {
                if (staff.Profile.ByteString != null)
                {
                    byte[] doc = Convert.FromBase64String(staff.Profile.ByteString);
                    if (doc != null)
                    {
                        string extension = Path.GetExtension(staff.Profile.FileName);
                        if (!string.IsNullOrEmpty(extension))
                        {
                            string filePath = "staff/profile/profile_" + DateTime.Now.Ticks.ToString() + extension;
                            fTPHelper.UploadFile(_Ftp.Server, _Ftp.UserName, _Ftp.Password, filePath, doc);
                            profileImage = filePath;
                        }
                    }
                }
                else
                {
                    if (staff.Profile.FileName != null)
                    {
                        profileImage = staff.Profile.FileName;
                    }
                }
            }

            if (staff.Documents != null && staff.Documents.Count > 0)
            {
                var docList = new List<string>();
                foreach (var item in staff.Documents)
                {
                    if (item.ByteString != null)
                    {
                        byte[] doc = Convert.FromBase64String(item.ByteString);
                        if (doc != null)
                        {
                            string extension = Path.GetExtension(item.FileName);
                            if (!string.IsNullOrEmpty(extension))
                            {
                                string filePath = "staff/document_" + DateTime.Now.Ticks.ToString() + "." + extension;
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
                    fileDocument = string.Join(",", docList);
                }
            }

            parameters.Add("@ProfileImage", profileImage);
            parameters.Add("@FileDocuments", fileDocument);

            var result = await _dapperHelper.Get<int>("usp_Staff_AddUpdate", parameters);
            responseModel.Data = result;
            //if (result == -1)
            //{
            //    responseModel.Status = false;
            //    responseModel.Message.Add("Staff Email Already exist in system.Please try with different email.");
            //}            
            //else if (result == 1)
            //{
            //    responseModel.Status = true;
            //    responseModel.Message.Add("Staff Added successfully.");
            //}
            //else if (result == 2)
            //{
            //    responseModel.Status = true;
            //    responseModel.Message.Add("Staff updated successfully.");
            //}
            //else
            //{
            //    responseModel.Status = true;
            //    responseModel.Message.Add("Error occured in system.Please contact administrator");
            //    responseModel.Data = result;
            //}
            //responseModel.Status = true;
            //responseModel.Message.Add("Success");
            responseModel.Data = result;
            return responseModel;
        }

        public async Task<ResponseModel<List<StaffListDTO>>> GetList(int pageNo, int pageSize, string? searchText, string userName)
        {
            ResponseModel<List<StaffListDTO>> responseModel = new ResponseModel<List<StaffListDTO>>();
            var parameters = new DynamicParameters();
            parameters.Add("@PageNo", pageNo);
            parameters.Add("@PageSize", pageSize);
            parameters.Add("@SearchText", searchText);
            parameters.Add("@UserName", userName);
            var result = await _dapperHelper.GetAll<StaffListDTO>("usp_Staff_GetList", parameters);

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

        public async Task<ResponseModel<StaffDTO>> GetByID(string pid, string userName)
        {
            ResponseModel<StaffDTO> responseModel = new ResponseModel<StaffDTO>();
            var parameters = new DynamicParameters();
            parameters.Add("@Pid", pid);
            parameters.Add("@UserName", userName);
            var result = await _dapperHelper.Get<StaffDTO>("usp_Staff_GetByID", parameters);
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
            var result = await _dapperHelper.Get<int>("usp_Staff_ActivateDeactivate", parameters);
            //responseModel.Status = true;
            //responseModel.Message.Add("Success");
            responseModel.Data = result;
            return responseModel;
        }

        public async Task<int> UpdateStaffUser(string pid, string userName)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Pid", pid);
            parameters.Add("@UserName", userName);
            var result = await _dapperHelper.Get<int>("usp_Staff_ActivateDeactivate", parameters);
            return result;
        }
    }
}
