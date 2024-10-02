using Application.BaseManager;
using Application.Models;
using Application.Models.Permissions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace SkyLearn.Portal.Api.Services
{
    public class MenuService : RestService
    {
        private readonly Context _context;
        private readonly IMapper _mapper;
        public MenuService(Context context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<IList<Menu>> GetMenuByRole(List<int> roleIds, string targetArea = "")
        {
            try
            {
                var query = (from ca in _context.ControllerActions
                             join ap in _context.ActionPermissions
                             on ca.Id equals ap.ControllerActionId into joinedData
                             from subap in joinedData.DefaultIfEmpty()
                             where roleIds.Contains(subap.RoleId) &&
                                ca.Area == targetArea
                             group ca by ca.Controller into controllerGroup
                             select new Menu
                             {
                                 Title = controllerGroup.Key
                             }).ToList();

                return query;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
