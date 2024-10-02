using Application.BaseManager;
using Application.Helpers;
using Application.Models;
using Application.Response;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SkyLearn.ContentPreview.Api.Services;

namespace SkyLearn.ContentPreview.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FormDataContentController : BaseAPIController
    {
        private readonly IMapper _mapper;
        private readonly FormDataContentService _formDataContentService;
        private readonly FormContentService _form_service;
        private readonly IHttpContextAccessor _contextAccessor;
        public FormDataContentController(APIResponse aPIResponse, ILogger<FormDataContentController> _logger, IMapper mapper, FormDataContentService formDataContentService, FormContentService form_service, IHttpContextAccessor contextAccessor) : base(aPIResponse, _logger, contextAccessor)
        {
            _mapper = mapper;
            _formDataContentService = formDataContentService;
            _form_service = form_service;
            _contextAccessor = contextAccessor;
        }

        [HttpPost]
        public async Task<ActionResult<FormDataDTO>> PostFormData([FromBody] CreateFormDataDTO formData)
        {
            try
            {
                var component = await _form_service.Retrieve<Form>(formData.FormPid);
                if (component == null)
                {
                    return OnNotFound("Form not found", "Not found");
                }
                var comp_data = _mapper.Map<FormData>(formData);
                comp_data.Pid = AppHelper.GeneratePid(Constant.PREFIX_SEMESTER);
                comp_data.CreatedAt = DateTime.UtcNow;
                comp_data.Form = component;
                comp_data.FormsData = formData.Data;
                var data = _mapper.Map<FormDataDTO>(await _formDataContentService.Create(comp_data));
                return this.OnSuccess(data, 200);
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error while Inserting Form data {0}", ex);
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}
