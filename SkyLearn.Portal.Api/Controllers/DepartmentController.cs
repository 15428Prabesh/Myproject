using Application.Helpers;
using Application.Models;
using Application.Response;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Application.BaseManager;
using SkyLearn.Portal.Api.Service;
using SkyLearn.Portal.Api.Services;
using System.Net;
using SkyLearn.Portal.Api.Middleware;

namespace SkyLearn.Portal.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [TypeFilter(typeof(AuthorizedUser))]
    public class DepartmentController : BaseAPIController
    {

        private readonly IMapper _mapper;
        private readonly DepartmentService _departmentService;
        private readonly IHttpContextAccessor httpContextAccessor;
        public DepartmentController(APIResponse aPIResponse, ILogger<DepartmentController> _logger, IMapper mapper, DepartmentService departmentService, IHttpContextAccessor _httpContextAccessor) : base(aPIResponse, _logger, _httpContextAccessor)
        {
            _mapper = mapper;
            _departmentService = departmentService;
            httpContextAccessor = _httpContextAccessor;
        }
        [HttpGet]
        public async Task<ActionResult<List<DepartmentDTO>>> GetAllDepartments(bool paginate = false, int pageSize = 10, int pageNumber = 1, string filterKey = "", string filterValue = "", string sortColumn = null, bool sortAsc = false)
        {
            try
            {
                var data = await _departmentService.List<Department, DepartmentDTO>(paginate, pageSize, pageNumber, filterKey, filterValue, sortColumn, sortAsc);
                return this.OnSuccess(data, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error while fetching department list {0}", ex);
                return StatusCode((int)HttpStatusCode.InternalServerError, "Internal Server Error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<DepartmentDTO>> CreateDepartment(CreateDepartmentDTO fields)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string uniqueColumn = nameof(Department.DepartmentName);
                    var model = _mapper.Map<Department>(fields);
                    model.Pid = AppHelper.GeneratePid(Constant.PREFIX_DEPARTMENT);
                    model.CreatedAt = DateTime.UtcNow;
                    model.CreatedBy = CurrentUserName;
                    var res = _mapper.Map<DepartmentDTO>(await _departmentService.Create<Department>(model, uniqueColumn, model.DepartmentName));
                    if (res.Id > 0)
                        return this.OnSuccess(res, (int)HttpStatusCode.OK);
                    else return this.OnBadRequest("Duplicate " + uniqueColumn, HttpStatusCode.Conflict.ToString(), (int)HttpStatusCode.Conflict);
                }
                return this.OnBadRequest("Please enter all the required fields", "validation", (int)HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error while creating a department . {0}", ex);
                return StatusCode((int)HttpStatusCode.InternalServerError, "Internal server error");
            }

        }

        [HttpGet("{Pid}")]

        public async Task<ActionResult<DepartmentDTO>> GetDetails(string Pid)
        {
            try
            {
                Department data = await _departmentService.Retrieve<Department>(Pid);
                if (data != null)
                {
                    var dto_data = _mapper.Map<DepartmentDTO>(data);
                    return this.OnSuccess(dto_data, (int)HttpStatusCode.OK);
                }
                return this.OnNotFound("Data not found", "error", (int)HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error while retrieving data . {0}", ex);
                return StatusCode((int)HttpStatusCode.InternalServerError, "Internal server error");
            }
        }
        [HttpPut("{Pid}")]
        public async Task<ActionResult<DepartmentDTO>> UpdateDepartment(string Pid, UpdateDepartmentDTO fields)
        {
            try
            {
                var data = await _departmentService.Retrieve<Department>(Pid);
                if (data != null)
                {
                    if (ModelState.IsValid)
                    {
                        data.UpdatedAt = DateTime.UtcNow;
                        data.IsModified = true;
                        data.DepartmentName = fields.DepartmentName;
                        data.Type = fields.Type;
                        data.Descriptions = fields.Descriptions;
                        data.IsActive = fields.IsActive;
                        var res = _mapper.Map<DepartmentDTO>(await _departmentService.UpdatePut(Pid, data));
                        return this.OnSuccess(res, (int)HttpStatusCode.OK);
                    }
                }
                return this.OnNotFound("Department not found", "error", (int)HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error while Updating Department {0}", ex);
                return StatusCode((int)HttpStatusCode.InternalServerError, "Internal Server Error");
            }
        }

        [HttpDelete("{Pid}")]
        public async Task<ActionResult<string>> DeleteDepartment(string Pid)
        {
            try
            {
                var instance = await _departmentService.Retrieve<Department>(Pid);
                if (instance == null)
                {
                    return this.OnNotFound("Department Not Found", "not found", (int)HttpStatusCode.NotFound);
                }
                var res = await _departmentService.Delete(instance, false);
                return this.OnSuccess("Department Deleted successfully", (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error while Deleting Department {0}", ex);
                return StatusCode((int)HttpStatusCode.InternalServerError, "Internal Server Error");
            }
        }

    }
}
