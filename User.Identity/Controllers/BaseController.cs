using Microsoft.AspNetCore.Mvc;
using User.Identity.Entities;

namespace User.Identity.Controllers
{
    public class BaseController:ControllerBase
    {
        // returns the current authenticated account (null if not logged in)
        public Account Account => (Account)HttpContext.Items["Account"];
    }
}
