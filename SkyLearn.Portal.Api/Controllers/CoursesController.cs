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

namespace SkyLearn.Portal.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [TypeFilter(typeof(AuthorizedUser))]
    public class CoursesController : BaseAPIController
    {
        private readonly IMapper _mapper;
        private readonly CoursesService _coursesService;
        private readonly DepartmentService _departmentService;
        private readonly SemesterService _semesterService;
        private readonly IHttpContextAccessor httpContextAccessor;
        public CoursesController(APIResponse aPIResponse, ILogger<CoursesController> _logger, IMapper mapper, CoursesService coursesService,DepartmentService departmentService,SemesterService semesterService, IHttpContextAccessor _httpContextAccessor) : base(aPIResponse, _logger, _httpContextAccessor)
        {
            _mapper = mapper;
            _coursesService = coursesService;
            _departmentService = departmentService;
            _semesterService = semesterService;
            httpContextAccessor = _httpContextAccessor;
        }
        [HttpGet]
     
        public async Task<ActionResult<List<CourseDTO>>> GetAllCourses(bool paginate=false,int pageSize=10,int pageNumber=1)
        {
            try
            {
                var data = await _coursesService.List<Courses,CourseDTO>(paginate,pageSize,pageNumber);
                return this.OnSuccess(data, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error while fetching Courses {0}", ex);
                return StatusCode((int)HttpStatusCode.InternalServerError, "Internal Server Error");
            }
        }
        [HttpPost]
        public async Task<ActionResult<CourseDTO>> CreateCourse(CreateCoursesDTO fields)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var department_instance = await _departmentService.Retrieve<Department>(fields.DepartmentPid);
                    var semester_instance = await _semesterService.Retrieve<Semester>(fields.SemesterPid);
                    if(department_instance==null)
                        return this.OnBadRequest("Invalid Department ID","validation");
                    if(semester_instance == null)
                        return this.OnBadRequest("Invalid Semester ID", "validation");
                    var model = _mapper.Map<Courses>(fields);
                    model.Pid = AppHelper.GeneratePid(Constant.PREFIX_COURSES);
                    model.CreatedAt = DateTime.UtcNow;
                    model.CreatedBy = CurrentUserName;
                    model.Semester = semester_instance;
                    model.Department = department_instance;
                    var data = _mapper.Map<CourseDTO>(await _coursesService.Create(model));
                    return this.OnSuccess(data, (int)HttpStatusCode.OK);
                }
                return this.OnBadRequest("Fill all the required fields", "validation", (int)HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error while creating Course.{0}", ex);
                return StatusCode((int)HttpStatusCode.InternalServerError, "Internal Server Error");
            }
        }
        [HttpGet("{Pid}")]
        public async Task<ActionResult<CourseDTO>> GetDetails(string Pid)
        {
            try
            {
                Courses data = await _coursesService.Retrieve<Courses>(Pid);
                if (data != null)
                {
                    var res = _mapper.Map<CourseDetailDTO>(data);
                    return this.OnSuccess(res, (int)HttpStatusCode.OK);
                }
                return this.OnNotFound("Courses not found", "error", (int)HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error while retrieving details . {0}", ex);
                return StatusCode((int)HttpStatusCode.InternalServerError, "Internal server error");
            }
        }

        [HttpPut("{Pid}")]
        public async Task<ActionResult<CourseDTO>> UpdateCourse(string Pid, UpdateCoursesDTO fields)
        {
            try
            {
                var data = await _coursesService.Retrieve<Courses>(Pid);
                if (data != null)
                 {
                    if (ModelState.IsValid)
                    {
                        if (data.SemesterId !=0)
                        {
                            var semester = await _semesterService.Retrieve<Semester>(fields.SemesterPid);
                            var department = await _departmentService.Retrieve<Department>(fields.DepartmentPid);
                            if (semester == null)
                            {
                                return this.OnNotFound("Invalid Semester", "error", (int)HttpStatusCode.NotFound);
                            }
                            if (department == null)
                            {
                                return this.OnNotFound("Invalid Department", "error", (int)HttpStatusCode.NotFound);
                            }
                            data.Semester = semester;
                            data.Department = department;
                        }
                        data.UpdatedAt = DateTime.UtcNow;
                        data.IsModified = true;
                        data.Title = fields.Title;
                        data.Details = fields.Details;
                        data.Features = fields.Features;
                        data.IsActive = fields.IsActive;
                        var res = _mapper.Map<CourseDTO>(await _coursesService.UpdatePut(Pid, data));
                        return this.OnSuccess(res, (int)HttpStatusCode.OK);
                    }
                }
                return this.OnNotFound("Course not found", "error", (int)HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error while Updating Course {0}", ex);
                return StatusCode((int)HttpStatusCode.InternalServerError, "Internal Server Error");
            }
        }

        [HttpDelete("{Pid}")]
        public async Task<ActionResult<string>> DeleteCourse(string Pid)
        {
            try
            {
                var instance = await _coursesService.Retrieve<Courses>(Pid);
                if (instance == null)
                {
                    return this.OnNotFound("Course Not Found", "not found", 404);
                }
                var res = await _coursesService.Delete(instance, false);
                return this.OnSuccess("Course Deleted successfully", 200);
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error while Deleting Course {0}", ex);
                return StatusCode((int)HttpStatusCode.InternalServerError, "Internal Server Error");
            }

        }
    }
}
