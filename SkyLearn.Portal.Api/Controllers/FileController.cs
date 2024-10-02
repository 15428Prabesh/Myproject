using Core.Helper.APiCall;
using Core;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using SkyLearn.Portal.Api.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Security.Policy;
using System.Net;
using Core.Constants;
using Microsoft.Extensions.Options;
using SkyLearn.Portal.Api.Middleware;

namespace SkyLearn.Portal.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [TypeFilter(typeof(AuthorizedUser))]
    public class FileController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;
        private readonly ApiClient _apiClient;
        private readonly string _fileName;
        private readonly string _filePath;
        private readonly string _fileExtension;
        private readonly string _fileNameExtension;
        private IHostingEnvironment _environment;
        private readonly BaseUrl _baseUrl;
        public FileController(HttpClient httpClient, ApiClient apiClient, IOptions<BaseUrl> baseUrl, IHostingEnvironment hostingEnvironment)
        {
            _httpClient = httpClient;
            _apiClient = apiClient;
            _environment = hostingEnvironment;
            _baseUrl = baseUrl.Value;
        }
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            var location = await UploadAsync(file);
            return new JsonResult(new
            {
                location = location
            });
        }
        private async Task<string> UploadAsync(IFormFile file)
        {
            string uniqueFileName = "";
            string fullFilePath = "";
            string originHeader = Request.Headers["Origin"];
            string domainWithoutPort = "";
            if (!string.IsNullOrEmpty(originHeader))
            {
                Uri originUri = new Uri(originHeader);
                domainWithoutPort = originUri.Host;
                // 'domainWithoutPort' will now contain the domain without the port.
            }
            domainWithoutPort = DomainExtractor.ExtractMainDomain(originHeader);
            string wwwroot = "wwwroot";
            string fileRootPath = "resources";
            string imageFolderPath = "images";
            if (file != null)
            {
                if (_environment.WebRootPath == null)
                {
                    _environment.WebRootPath = Path.Combine(_environment.ContentRootPath, wwwroot);
                }
                _environment.WebRootPath = Path.Combine(_environment.ContentRootPath, wwwroot);
                _environment.WebRootPath = Path.Combine(_environment.WebRootPath, fileRootPath);
                string uploadFilePath = Path.Combine(_environment.WebRootPath, domainWithoutPort);
                uploadFilePath = Path.Combine(uploadFilePath, imageFolderPath);

                if (!Directory.Exists(uploadFilePath))
                    Directory.CreateDirectory(uploadFilePath);

                uniqueFileName = MediaHelper.NewFileName(file.FileName, uploadFilePath);
                fullFilePath = Path.Combine(uploadFilePath, uniqueFileName);
                using (var stream = new FileStream(fullFilePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }

            return _baseUrl.PortalBaseUrl + (fileRootPath + "/" + domainWithoutPort + "/" + imageFolderPath + "/" + uniqueFileName);
            //return fullFilePath;
        }

    }
}
