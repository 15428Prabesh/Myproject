using Application.Helpers;
using Application.Models;
using Application.Response;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Application.BaseManager;
using SkyLearn.Portal.Api.Services;
using SkyLearn.Portal.Api.Middleware;
using System.Net;
using SkyLearn.Portal.Api.Interfaces;
using System.Drawing.Printing;
using Core.Constants;
using Core.Helper.APiCall;
using Core;
using Microsoft.Extensions.Options;

namespace SkyLearn.Portal.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [TypeFilter(typeof(AuthorizedUser))]
    public class StaffController : BaseAPIController
    {
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly DepartmentService departmentService;
        private readonly IStaffDapperService _staffDapperService;
        private readonly UserController _userController;
        public StaffController(APIResponse aPIResponse, ILogger<StaffController> _logger, IMapper mapper, IHttpContextAccessor httpContextAccessor, IStaffDapperService staffDapperService, UserController userController) : base(aPIResponse, _logger, httpContextAccessor)
        {
            _mapper = mapper;
            _staffDapperService = staffDapperService;
            _httpContextAccessor = httpContextAccessor;
            _userController = userController;
        }

        [HttpGet]
        public async Task<ActionResult<List<StaffListDTO>>> GetAllStaffs(string? searchText, int pageSize = 10, int pageNumber = 1)
        {
            var data = await _staffDapperService.GetList(pageNumber, pageSize, searchText, CurrentUserID);
            return this.OnSuccess(data, (int)HttpStatusCode.OK);
        }

        [HttpGet("{Pid}")]
        public async Task<ActionResult<StaffDTO>> GetByID(string Pid)
        {

            var data = await _staffDapperService.GetByID(Pid, CurrentUserID);
            if (data != null && data.Data != null)
            {
                return this.OnSuccess(data?.Data, (int)HttpStatusCode.OK);
            }
            else
                return this.OnBadRequest("Data Not Found.", "validation error", (int)HttpStatusCode.BadRequest);

        }

        [HttpPost]
        public async Task<ActionResult<ResponseModel<int>>> CreateStaff(Staff staff)
        {
            string id = AppHelper.GeneratePid(Constant.PREFIX_STAFF);

            var data = await _staffDapperService.AddUpdate(id, staff, CurrentUserID);
            if (data.Data == -1)
            {
                return this.OnBadRequest("Staff Email Already exist in system.Please try with different email.", "validation", (int)HttpStatusCode.BadRequest);

            }
            else if (data.Data == 1)
            {
                var user = _mapper.Map<CreateUserDTO>(staff);
                await _userController.UserCreation(user);
                return this.OnSuccess(data, (int)HttpStatusCode.OK, "Staff Added successfully.");
            }
            else if (data.Data == 2)
            {
                return this.OnSuccess(data, (int)HttpStatusCode.OK, "Staff updated successfully.");
            }
            else
            {
                return this.OnBadRequest("Error occured in system.Please contact administrator.", "internal server error", (int)HttpStatusCode.BadRequest);
            }


        }

        [HttpPut("id")]
        public async Task<ActionResult<ResponseModel<int>>> UpdateStaff(string id, Staff staff)
        {
            var data = await _staffDapperService.AddUpdate(id, staff, CurrentUserID);
            if (data.Data == -1)
            {
                return this.OnBadRequest("Staff Email Already exist in system.Please try with different email.", "validation", (int)HttpStatusCode.BadRequest);

            }
            else if (data.Data == 1)
            {
                return this.OnSuccess(data, (int)HttpStatusCode.OK, "Staff Added successfully.");
            }
            else if (data.Data == 2)
            {
                var staffObj = await _staffDapperService.GetByID(id, CurrentUserID);
                if (staffObj != null && staffObj.Data != null && !staffObj.Data.IsUserCreated)
                {
                    var user = _mapper.Map<CreateUserDTO>(staff);
                    await _userController.UserCreation(user);
                }

                return this.OnSuccess(data, (int)HttpStatusCode.OK, "Staff updated successfully.");
            }
            else
            {
                return this.OnBadRequest("Error occured in system.Please contact administrator.", "internal server error", (int)HttpStatusCode.BadRequest);
            }
        }

        [HttpPut("{id}/{isActive}")]
        public async Task<ActionResult<ResponseModel<int>>> ActivateDeactivate(string id, bool isActive)
        {
            var data = await _staffDapperService.ActivateDeactivate(id, isActive, CurrentUserID);
            var text = "activated";
            if (!isActive)
            {
                text = "deactivated";
            }
            if (data.Data == -1)
            {
                return this.OnBadRequest("Staff  Already " + text + ".", "validation", (int)HttpStatusCode.BadRequest);

            }
            else if (data.Data == 1)
            {
                return this.OnSuccess(data, (int)HttpStatusCode.OK, "Staff " + text + " successfully.");
            }
            else
            {
                return this.OnBadRequest("Error occured in system.Please contact administrator.", "internal server error", (int)HttpStatusCode.BadRequest);
            }
        }

        [HttpGet("department")]
        public async Task<ActionResult<List<DepartmentDTO>>> GetAllDepartments(bool paginate = false, int pageSize = 10, int pageNumber = 1, string filterKey = "", string filterValue = "", string sortColumn = null, bool sortAsc = false)
        {
            try
            {
                var data = await departmentService.List<Department, DepartmentDTO>(paginate, pageSize, pageNumber, filterKey, filterValue, sortColumn, sortAsc);
                return this.OnSuccess(data, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error while fetching department list {0}", ex);
                return StatusCode((int)HttpStatusCode.InternalServerError, "Internal Server Error");
            }
        }

    }
}
