using Application.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using SkyLearn.Portal.Api.Middleware;
using SkyLearn.Portal.Api.Services;
using System.Reflection;
using System.Security.Cryptography;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SkyLearn.Portal.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [TypeFilter(typeof(AuthorizedUser))]
    public class PageActionController : ControllerBase
    {
        private readonly PageActionService _permissionService;
        public PageActionController(PageActionService permissionService)
        {
            _permissionService = permissionService;

        }
        [HttpGet]
        public async Task<ActionResult<List<ControllerAction>>> GetAll()
        {
            List<ControllerAction> actions = GetAreaControllerNames();
            //List<ControllerAction> actions = GetReflectionActions();
            foreach (ControllerAction action in actions)
            {
                if (!string.IsNullOrEmpty(action.Controller))
                {
                    action.CreatedBy = "Sushil";
                    await _permissionService.AutomateActions(action);
                }
            }
            var groupedData = actions.GroupBy(
    d => new { d.Area, d.Controller, d.Pid },
    (key, group) => new
    {
        Area = key.Area,
        Controller = key.Controller,
        Pid = key.Pid,
        // Actions = group.Select(d => new { d.Action, d.Method }).ToList()
        Actions = group.GroupBy(
            item => item.Method,
            (keyGroup, groupGroup) => new
            {
                Method = keyGroup,
                Actions = groupGroup.Select(x => x.Action).ToList()
            }).ToList()
    });
            return new JsonResult(groupedData);
            //return actions;
        }
        [NonAction]
        private List<ControllerAction> GetAreaControllerNames()
        {
            List<ControllerAction> areaControllerNames = new List<ControllerAction>();

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                try
                {
                    var typesInAssembly = assembly.GetTypes();

                    var controllers = typesInAssembly
                        .Where(type => typeof(ControllerBase).IsAssignableFrom(type))
                        .Select(type => new
                        {
                            Area = type.GetCustomAttributes(typeof(AreaAttribute), false)
                               .Cast<AreaAttribute>()
                               .FirstOrDefault()?.RouteValue,
                            Controller = type.Name.Replace("Controller", "")
                        }).ToList();

                    foreach (var controller in controllers)
                    {
                        areaControllerNames.Add(new ControllerAction()
                        {
                            Pid = AppHelper.GeneratePid(Constant.PREFIX_ControllerAction),
                            Area = string.IsNullOrEmpty(controller.Area) ? string.Empty : controller.Area,
                            Controller = controller.Controller,
                            Method = string.Empty,
                            Action = string.Empty,
                            CreatedAt = DateTime.Now,
                            CreatedBy = "supersuer",
                            IsActive = true
                        });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing assembly {assembly.FullName}: {ex.Message}");
                }
            }

            return areaControllerNames;
        }



        [NonAction]
        private List<ControllerAction> GetReflectionActions()
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            List<Assembly> myAssemblies = new List<Assembly>();

            foreach (Assembly assembly in assemblies)
            {
                try
                {
                    var typesInAssembly = assembly.GetTypes();

                    if (typesInAssembly.Any(type => type.IsSubclassOf(typeof(ControllerBase))))
                    {
                        myAssemblies.Add(assembly);
                    }
                }
                catch (ReflectionTypeLoadException ex)
                {
                    Console.WriteLine($"Error loading types from assembly {assembly.FullName}: {ex.Message}");
                    foreach (var loaderException in ex.LoaderExceptions)
                    {
                        Console.WriteLine($"Loader Exception: {loaderException.Message}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing assembly {assembly.FullName}: {ex.Message}");
                }
            }

            var list = new List<ControllerAction>();
            foreach (Assembly asm in myAssemblies)
            {
                //  Assembly asm = Assembly.GetExecutingAssembly();

                var controlleractionlist = asm.GetTypes()
            .Where(type => typeof(ControllerBase).IsAssignableFrom(type))
            .SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
            .Where(method => method.IsPublic && !method.IsDefined(typeof(NonActionAttribute)))
            .Select(x =>
            new
            {
                Controller = x.DeclaringType.Name.Replace("Controller", ""),
                Action = x.Name,
                ReturnType = x.ReturnType.Name,
                Method = x.GetCustomAttributes()
            .FirstOrDefault(a => a is HttpMethodAttribute)?.GetType().Name.Replace("Attribute", ""),
                Area = x.DeclaringType.CustomAttributes.Where(c => c.AttributeType == typeof(AreaAttribute))
            }).ToList();

                foreach (var item in controlleractionlist)
                {
                    if (item.Area.Count() != 0)
                    {
                        list.Add(new ControllerAction()
                        {
                            Pid = AppHelper.GeneratePid(Constant.PREFIX_ControllerAction),
                            Controller = item.Controller,
                            Action = item.Action,
                            Method = item.Method,
                            Area = item.Area.Select(v => v.ConstructorArguments[0].Value.ToString()).FirstOrDefault()
                        });
                    }
                    else
                    {
                        list.Add(new ControllerAction()
                        {
                            Pid = AppHelper.GeneratePid(Constant.PREFIX_ControllerAction),
                            Controller = item.Controller,
                            Action = item.Action,
                            Method = item.Method,
                            Area = string.Empty
                        });
                    }
                }
            }

            list.OrderBy(x => x.Area).ThenBy(x => x.Controller).ThenBy(x => x.Action);
            return list;

        }
    }
}
