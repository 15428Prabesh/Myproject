using Application.BaseManager;
using Application.Helpers;
using Application.Models;
using Application.Response;
using AutoMapper;
using Core.Constants;
using Core.Helper.APiCall;
using Core;
using Microsoft.AspNetCore.Mvc;
using SkyLearn.Portal.Api.Interfaces;
using SkyLearn.Portal.Api.Middleware;
using SkyLearn.Portal.Api.Services;
using System.Net;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;

namespace SkyLearn.Portal.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //  [TypeFilter(typeof(AuthorizedUser))]
    public class StudentController : BaseAPIController
    {
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStudentDapperService _studentDapperService;
        private readonly UserController _userController;
        private readonly DepartmentService departmentService;
        private readonly SemesterService semesterService;
        public StudentController(APIResponse aPIResponse, ILogger<StudentController> _logger, IMapper mapper, IStudentDapperService studentDapperService, IHttpContextAccessor httpContextAccessor, DepartmentService _departmentService, UserController userController, SemesterService _semesterService) : base(aPIResponse, _logger, httpContextAccessor)
        {
            _mapper = mapper;
            _studentDapperService = studentDapperService;
            _httpContextAccessor = httpContextAccessor;
            _userController = userController;
            this.departmentService = _departmentService;
            this.semesterService = _semesterService;
        }
        [HttpPost]
        public async Task<ActionResult<ResponseModel<int>>> CreateStudent(Student student)
        {
            string id = AppHelper.GeneratePid(Constant.PREFIX_STUDENT);
            var data = await _studentDapperService.AddUpdate(id, student, CurrentUserID);


            if (data.Data == -1)
            {
                return this.OnBadRequest("Student Email Already exist in system.Please try with different email.", "validation", (int)HttpStatusCode.BadRequest);

            }
            else if (data.Data == -2)
            {
                return this.OnBadRequest("Parent Email Already exist in system.Please try with different email.", "validation", (int)HttpStatusCode.BadRequest);

            }
            else if (data.Data == 1)
            {
                var user = _mapper.Map<CreateUserDTO>(student);
                await _userController.UserCreation(user);
                await _userController.UserCreation(new CreateUserDTO
                {
                    Email = student.ParentEmail,
                    FirstName = student.FatherName,
                    LastName = string.Empty
                });
                return this.OnSuccess(data, (int)HttpStatusCode.OK, "Student Added successfully.");
            }
            else if (data.Data == 2)
            {
                return this.OnSuccess(data, (int)HttpStatusCode.OK, "Student updated successfully.");
            }
            else
            {
                return this.OnBadRequest("Error occured in system.Please contact administrator.", "internal server error", (int)HttpStatusCode.BadRequest);
            }

        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ResponseModel<int>>> UpdateStudent(string id, Student student)
        {
            var data = await _studentDapperService.AddUpdate(id, student, CurrentUserID);

            if (data.Data == -1)
            {
                return this.OnBadRequest("Student Email Already exist in system.Please try with different email.", "validation", (int)HttpStatusCode.BadRequest);

            }
            else if (data.Data == -2)
            {
                return this.OnBadRequest("Parent Email Already exist in system.Please try with different email.", "validation", (int)HttpStatusCode.BadRequest);

            }
            else if (data.Data == 1)
            {
                return this.OnSuccess(data, (int)HttpStatusCode.OK, "Student Added successfully.");
            }
            else if (data.Data == 2)
            {
                if (data.Data > 0)
                {
                    var studentObj = await _studentDapperService.GetByID(id, CurrentUserID);
                    if (studentObj != null && studentObj.Data != null)
                    {
                        if (!studentObj.Data.IsUserCreated)
                        {
                            var user = _mapper.Map<CreateUserDTO>(student);
                            await _userController.UserCreation(user);
                        }
                        if (!studentObj.Data.IsParentUserCreated)
                        {
                            await _userController.UserCreation(new CreateUserDTO
                            {
                                Email = student.ParentEmail,
                                FirstName = student.FatherName,
                                LastName = string.Empty
                            });
                        }
                    }
                }
                return this.OnSuccess(data, (int)HttpStatusCode.OK, "Student updated successfully.");
            }
            else
            {
                return this.OnBadRequest("Error occured in system.Please contact administrator.", "internal server error", (int)HttpStatusCode.BadRequest);
            }

        }

        [HttpGet]
        public async Task<ActionResult<List<StudentListDTO>>> GetAllStudents(string? searchText, bool paginate = false, int pageSize = 10, int pageNumber = 1)
        {
            var data = await _studentDapperService.GetList(pageNumber, pageSize, searchText, CurrentUserID);
            return this.OnSuccess(data, (int)HttpStatusCode.OK);
        }

        [HttpGet("{Pid}")]
        public async Task<ActionResult<StudentDTO>> GetDetails(string Pid)
        {
            var data = await _studentDapperService.GetByID(Pid, CurrentUserID);
            if (data != null && data.Data != null)
            {
                return this.OnSuccess(data?.Data, (int)HttpStatusCode.OK);
            }
            else
                return this.OnBadRequest("Data Not Found.", "validation error", (int)HttpStatusCode.BadRequest);

        }

        [HttpPut("activatedeactivate/{id}/{isActive}")]
        public async Task<ActionResult<ResponseModel<int>>> ActivateDeactivate(string id, bool isActive)
        {
            var data = await _studentDapperService.ActivateDeactivate(id, isActive, CurrentUserID);
            var text = "activated";
            if (!isActive)
            {
                text = "deactivated";
            }
            if (data.Data == -1)
            {
                return this.OnBadRequest("Student  Already " + text + ".", "validation", (int)HttpStatusCode.BadRequest);

            }
            else if (data.Data == 1)
            {
                return this.OnSuccess(data, (int)HttpStatusCode.OK, "Student " + text + " successfully.");
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

        [HttpGet("semester")]
        public async Task<ActionResult<List<SemesterListDTO>>> GetAllSemester(bool paginate = false, int pageSize = 10, int pageNumber = 1)
        {
            try
            {
                var semester_dto = await semesterService.List<Semester, SemesterListDTO>(paginate, pageSize, pageNumber);
                return this.OnSuccess(semester_dto);

            }
            catch (Exception ex)
            {
                this._logger.LogError("Error while getting Semester list.{0}", ex);
                return StatusCode((int)HttpStatusCode.InternalServerError, "Internal Server Error");
            }
        }

        //[HttpPut("upload")]
        //[AllowAnonymous]
        //public async Task<IActionResult> UploadFile(IFormFile file)
        //{
        //    var data = _studentDapperService.Upload(file);
        //    return this.OnSuccess(data, (int)HttpStatusCode.OK);
        //}

    }
}