using Application.Helpers;
using Application.Models;
using Application.Response;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
using Application.BaseManager;
using SkyLearn.Portal.Api.Services;
using Microsoft.AspNetCore.JsonPatch;
using System.Net;
using SkyLearn.Portal.Api.Middleware;
using Microsoft.AspNetCore.Http;

namespace SkyLearn.Portal.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [TypeFilter(typeof(AuthorizedUser))]
    public class SemesterController : BaseAPIController
    {
        private readonly IMapper _mapper;
        private readonly SemesterService semesterService;
        private readonly DepartmentService departmentService;
        private readonly IHttpContextAccessor httpContextAccessor;
        public SemesterController(APIResponse aPIResponse, ILogger<SemesterController> _logger,IMapper mapper,SemesterService _semesterService,DepartmentService _departmentService, IHttpContextAccessor _httpContextAccessor) : base(aPIResponse, _logger,_httpContextAccessor)
        {
            _mapper = mapper;
            semesterService= _semesterService;
            departmentService = _departmentService;
            httpContextAccessor = _httpContextAccessor;
        }

        [HttpGet]
        public async Task<ActionResult<List<SemesterListDTO>>> GetAllSemester(bool paginate=false,int pageSize=10,int pageNumber=1)
        {
            try
            {
                var semester_dto = await semesterService.List<Semester, SemesterListDTO>(paginate,pageSize,pageNumber);
                 return this.OnSuccess(semester_dto);

            }
            catch(Exception ex)
            {
                this._logger.LogError("Error while getting Semester list.{0}", ex);
                return StatusCode((int)HttpStatusCode.InternalServerError, "Internal Server Error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<SemesterDTO>> CreateSemester(CreateSemesterDTO field)
        {
            try
            {
                if (ModelState.IsValid)
                {                  
                    var department =await departmentService.Retrieve<Department>(field.DepartmentPid);
                    var contentData = _mapper.Map<Semester>(field);
                    contentData.Pid = AppHelper.GeneratePid(Constant.PREFIX_SEMESTER);
                    contentData.CreatedAt = DateTime.UtcNow;
                    contentData.Department = department;
                    contentData.CreatedBy =CurrentUserName;
                    var data = _mapper.Map<SemesterDTO>(await semesterService.Create(contentData));
                    return this.OnSuccess(data, (int)HttpStatusCode.OK);
                }
                return this.OnBadRequest("Fill all the required forms", "validation");
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error while creating Semester.{0}", ex);
                return StatusCode((int)HttpStatusCode.InternalServerError, "Internal Server Error");
            }
        }

        [HttpGet("{Pid}")]
        public async Task<ActionResult<SemesterDTO>> GetDetail(string Pid)
        {
            try
            {
                var entity = await semesterService.Retrieve<Semester>(Pid);
                if (entity == null)
                {
                    return this.OnBadRequest("Invalid Semester pid", "validation", (int)HttpStatusCode.BadRequest);
                }
                var department =await departmentService.RetrieveByID<Department>(entity.DepartmentId);
                var department_dto = _mapper.Map<DepartmentDTO>(department);
                var data = _mapper.Map<SemesterDTO>(entity);
                data.department = department_dto;
                return this.OnSuccess(data);
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error while Obtaining Semester details {0}", ex);
                return StatusCode((int)HttpStatusCode.InternalServerError, "Internal Server Error");
            }
        }
        [HttpPut("{Pid}")]
        public async Task<ActionResult<SemesterDTO>> UpdateSemester(string Pid, UpdateSemesterDTO semester)
        {
            try
            {
                var data = await semesterService.Retrieve<Semester>(Pid);
                if (data != null)
                {
                    if (ModelState.IsValid)
                    {
                        if (!string.IsNullOrEmpty(data.DepartmentPid))
                        {
                            var dep = await semesterService.Retrieve<Department>(semester.DepartmentPid);
                            if (dep == null)
                            {
                                return this.OnNotFound("Invalid Department", "error", (int)HttpStatusCode.NotFound);
                            }
                            data.Department = dep;
                        }
                        data.UpdatedAt = DateTime.UtcNow;
                        data.IsModified = true;
                        data.Name = semester.Name;
                        data.IsActive = semester.IsActive;
                        var res = _mapper.Map<SemesterDTO>(await semesterService.UpdatePut(Pid, data));
                        return this.OnSuccess(res, (int)HttpStatusCode.OK);
                    }
                }
                return this.OnBadRequest("Enter required fields", "validation", (int)HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error while updating Semester", ex);
                throw ex;
            }
        }
    }
}
