using Application;
using Application.BaseManager;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace SkyLearn.Portal.Api.Services
{
    public class RolesService : RestService
    {
        private readonly Context _context;
        public RolesService(Context context, IMapper mapper) : base(context, mapper)
        {
        _context = context;
        }

        public  bool Exists(string name)
        {
           return  _context.Roles.Any(x => x.RoleName == name);
        }
        public async Task<bool> DeletePermissions(string roleId)
        {
            try
            {
                _context.ActionPermissions.Where(x => x.Roles.Pid == roleId).ExecuteDelete();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }           
        }
    }
}
