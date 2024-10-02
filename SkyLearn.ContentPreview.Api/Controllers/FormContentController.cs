using Application.BaseManager;
using Application.Helpers;
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
    public class FormContentController : BaseAPIController
    {
        private readonly IMapper _mapper;
        private readonly FormContentService _formContentService;
        private readonly IHttpContextAccessor _contextAccessor;
        public FormContentController(APIResponse aPIResponse, ILogger<FormContentController> _logger, IMapper mapper, FormContentService formContentService, IHttpContextAccessor contextAccessor) : base(aPIResponse, _logger, contextAccessor)
        {
            _mapper = mapper;
            _formContentService = formContentService;
            _contextAccessor = contextAccessor;
        }


        [HttpGet]
        public async Task<ActionResult<FormDTO>> GetFormByName([FromQuery] string name)
        {
            try
            {

                var component = await _formContentService.RetrieveByName<Form>(name);

                if (component == null)
                {
                    return this.OnNotFound("Form component not found", "error", 404);
                }

                var res = _mapper.Map<FormDTO>(component);
                return this.OnSuccess(res);
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error while Obtaining Form component {0}", ex);
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}
