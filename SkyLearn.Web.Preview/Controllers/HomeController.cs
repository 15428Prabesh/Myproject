using Core;
using Microsoft.AspNetCore.Mvc;
using RazorLight;
using SkyLearn.Web.Preview.Models;
using System.Diagnostics;
using System.Reflection;

namespace SkyLearn.Web.Preview.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            ApiResponse apiResponse = new ApiResponse();
            //string renderedContent = _razorEngine.CompileRenderAsync("Index.cshtml",apiResponse).Result;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}