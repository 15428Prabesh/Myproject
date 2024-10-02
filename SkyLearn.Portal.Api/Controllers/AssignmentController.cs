using Application.BaseManager;
using Application.Helpers;
using Application.Models;
using Application.Response;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SkyLearn.Portal.Api.Interfaces;
using SkyLearn.Portal.Api.Middleware;
using SkyLearn.Portal.Api.Services;
using System.Drawing.Printing;
using System.Net;
using System.Security.Cryptography;

namespace SkyLearn.Portal.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [TypeFilter(typeof(AuthorizedUser))]
    public class AssignmentController : BaseAPIController
    {
        private readonly IMapper _mapper;
        private readonly AssignmentService _assignmentService;
        private readonly CoursesService _coursesService;
        private readonly DepartmentService _departmentService;
        private readonly SemesterService _semesterService;
        private readonly StatusService _statusService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAssignmentEnrollService _assignmentEnrollService;

        public AssignmentController(APIResponse aPIResponse, ILogger<AssignmentController> _logger, IMapper mapper, AssignmentService assignmentService,CoursesService coursesService,DepartmentService departmentService,IHttpContextAccessor httpContextAccessor,SemesterService semesterService,StatusService statusService, IAssignmentEnrollService assignmentEnrollService) : base(aPIResponse, _logger, httpContextAccessor)
        {
            _mapper = mapper;
            _assignmentService = assignmentService;
            _coursesService = coursesService;
            _departmentService = departmentService;
            _httpContextAccessor = httpContextAccessor;
            _semesterService = semesterService;
            _statusService = statusService;
            _assignmentEnrollService = assignmentEnrollService;
        }
        [HttpGet]
        public async Task<ActionResult<List<AssignmentDTO>>> GetAllAssignments(bool paginate= false, int pageSize = 10, int pageNumber = 1)
        {
            try
            {
                var data = await _assignmentService.List<Assignment,AssignmentDTO>(paginate, pageSize, pageNumber);
                return this.OnSuccess(data, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error while fetching Assignments {0}", ex);
                return StatusCode(500, "Internal Server Error");
            }
        }
        [HttpPost]
        public async Task<ActionResult<AssignmentDTO>> CreateAssignment(CreateAssignmentDTO fields)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Core.Data user = (Core.Data?)_httpContextAccessor.HttpContext.Items["user"];
                    var course = await _coursesService.Retrieve<Courses>(fields.CoursePid);
                    var department = await _departmentService.Retrieve<Department>(fields.DepartmentPid);
                    var semester = await _semesterService.Retrieve<Semester>(fields.SemesterPid);
                    var model = _mapper.Map<Assignment>(fields);
                    model.Pid = AppHelper.GeneratePid(Constant.PREFIX_ASSIGNMENT);
                    model.CreatedAt = DateTime.UtcNow;
                    model.Course = course;
                    model.CreatedBy = user.Email.ToString();
                    model.Department= department;
                    model.Semester = semester;
                    model.StaffID = Convert.ToInt32(fields.StaffPid);
                    var data = _mapper.Map<AssignmentDTO>(await _assignmentService.Create(model));

                    return this.OnSuccess(data, (int)HttpStatusCode.OK);
                }
                return this.OnBadRequest("Fill all the required fields", "validation", (int)HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error while creating Assignment.{0}", (int)HttpStatusCode.OK);
                return StatusCode((int)HttpStatusCode.InternalServerError, "Internal Server Error");
            }
        }
        [HttpGet("{Pid}")]
        public async Task<ActionResult<AssignmentDTO>> GetDetails(string Pid)
        {
            try
            {
                Assignment data = await _assignmentService.Retrieve<Assignment>(Pid);
                if (data != null)
                {
                    var res = _mapper.Map<AssignmentDetailDTO>(data);
                    return this.OnSuccess(res, (int)HttpStatusCode.OK);
                }
                return this.OnNotFound("Data not found", "error", (int)HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error while retrieving details . {0}", ex);
                return StatusCode((int)HttpStatusCode.InternalServerError, "Internal server error");
            }
        }

        [HttpPut("{Pid}")]
        public async Task<ActionResult<AssignmentDTO>> UpdateAssignment(string Pid, UpdateAssignmentDTO fields)
        {
            try
            {
                var data = await _assignmentService.Retrieve<Assignment>(Pid);
                if (data != null)
                {
                    if (ModelState.IsValid)
                    {
                        if (!string.IsNullOrEmpty(data.CoursePid))
                        {
                            var courses = await _coursesService.Retrieve<Courses>(fields.CoursePid);
                            var department = await _departmentService.Retrieve<Department>(fields.DepartmentPid);
                            var semester = await _semesterService.Retrieve<Semester>(fields.SemesterPid);
                         //   var staff = await _staffService.Retrieve<Staff>(fields.StaffPid);
                            if (courses == null)
                            {
                                return this.OnNotFound("Invalid Course", "error", (int)HttpStatusCode.NotFound);
                            }
                            if (department == null)
                            {
                                return this.OnNotFound("Invalid Department", "error", (int)HttpStatusCode.NotFound);
                            }
                            if (semester == null)
                            {
                                return this.OnNotFound("Invalid Semester", "error", (int)HttpStatusCode.NotFound);
                            }
                            //if (staff == null)
                            //{
                            //    return this.OnNotFound("Invalid Staff", "error", (int)HttpStatusCode.NotFound);
                            //}
                            data.Course = courses;
                            data.Department = department;
                            data.Semester = semester;
                           // data.Staff = staff;
                        }
                        data.UpdatedAt = DateTime.UtcNow;
                        data.IsModified = true;
                        data.Marks = fields.Marks;
                        data.Name = fields.Name;
                        data.AssignmentDetails = fields.AssignmentDetails;
                        var res = _mapper.Map<AssignmentDTO>(await _assignmentService.UpdatePut(Pid, data));
                        return this.OnSuccess(res, (int)HttpStatusCode.OK);
                    }
                }
                return this.OnNotFound("Data not found", "error", (int)HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error while Updating Assignment {0}", ex);
                return StatusCode((int)HttpStatusCode.InternalServerError, "Internal Server Error");
            }
        }

        [HttpDelete("{Pid}")]
        public async Task<ActionResult<string>> DeleteAssignment(string Pid)
        {
            try
            {
                var instance = await _assignmentService.Retrieve<Assignment>(Pid);
                if (instance == null)
                {
                    return this.OnNotFound("Assignment Not Found", "not found", (int)HttpStatusCode.NotFound);
                }
                var res = await _assignmentService.Delete(instance, false);
                return this.OnSuccess("Assignment Deleted successfully", (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error while Deleting Assignment {0}", ex);
                return StatusCode((int)HttpStatusCode.InternalServerError, "Internal Server Error");
            }

        }

        [HttpPut("assignstudent")]
        public async Task<ActionResult<string>> AssignStudent(string assignmentID, List<string> studentList)
        {
            try
            {
                var result = await _assignmentEnrollService.AssignStudent(assignmentID, studentList,false, CurrentUserID);
                if (result.Data > 0)
                {
                    return this.OnSuccess(result, (int)HttpStatusCode.OK, "Student assigned successfully.");
                }
                else
                {
                    return this.OnBadRequest("Error occured in system.Please contact administrator.", "internal server error", (int)HttpStatusCode.BadRequest);

                }               
                
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error while Deleting Assignment {0}", ex);
                return StatusCode((int)HttpStatusCode.InternalServerError, "Internal Server Error");
            }

        }


    }
}
