using Application.BaseManager;
using Application.Models;
using Application.Response;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SkyLearn.ContentPreview.Api.Services;

namespace SkyLearn.ContentPreview.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SemesterContentController : BaseAPIController
    {
        private readonly IMapper _mapper;
        private readonly SemesterContentService _semesterContentService;
        private readonly DepartmentContentServices _departmentContentServices;
        private readonly IHttpContextAccessor _contextAccessor;
        public SemesterContentController(APIResponse aPIResponse, ILogger<SemesterContentController> _logger, IMapper mapper, SemesterContentService semesterContentService, DepartmentContentServices departmentContentServices, IHttpContextAccessor contextAccessor) : base(aPIResponse, _logger,contextAccessor)
        {
            _mapper = mapper;
            _semesterContentService = semesterContentService;
            _departmentContentServices = departmentContentServices;
            _contextAccessor = contextAccessor;
        }

        [HttpGet]
        public async Task<ActionResult<List<SemesterDTO>>> GetAllSemester()
        {
            try
            {
                var semester_dto = _mapper.Map<List<SemesterDTO>>(await _semesterContentService.List<Semester,SemesterDTO>());
                return this.OnSuccess(semester_dto);

            }
            catch (Exception ex)
            {
                this._logger.LogError("Error while getting Semester list.{0}", ex);
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("{Pid}")]
        public async Task<ActionResult<SemesterDTO>> GetDetail(string Pid)
        {
            try
            {
                var entity = await _semesterContentService.Retrieve<Semester>(Pid);
                if (entity == null)
                {
                    return this.OnBadRequest("Invalid Semester pid", "validation", 400);
                }
                var department = await _departmentContentServices.RetrieveByID<Department>(entity.DepartmentId);
                var department_dto = _mapper.Map<DepartmentDTO>(department);
                var data = _mapper.Map<SemesterDTO>(entity);
                data.department = department_dto;
                return this.OnSuccess(data);
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error while Obtaining Semester details {0}", ex);
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}
