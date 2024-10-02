using Application.Models.Permissions;
using AutoMapper;
using Application;
using Application.BaseManager;
using Application.Models;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using System.Text.RegularExpressions;
using System.Net;
using Microsoft.Bot.Schema.Teams;
using System.Web.Razor.Tokenizer.Symbols;

namespace SkyLearn.Portal.Api.Services
{
    public class PermissionService : RestService
    {
        private readonly Context _context;
        private readonly IMapper _mapper;
        public PermissionService(Context context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<IList<ControllerActionDto>> GetAllPermissions(int roleId, string keyword)
        {
            var query = from ca in _context.ControllerActions
                        join ap in _context.ActionPermissions
                        on ca.Id equals ap.ControllerActionId into joinedData
                        from subap in joinedData.DefaultIfEmpty()
                        where subap == null || subap.RoleId == roleId
                        select new ControllerActionDto
                        {
                            Pid = ca.Pid,
                            Id = ca.Id,
                            Controller = ca.Controller,
                            Action = ca.Action,
                            Area = ca.Area,
                            Method = ca.Method,
                            HasPermission = subap != null
                        };

            query = query.Where(x => x.Controller.Contains(keyword) || x.Method.Contains((keyword)) || x.Action.Contains(keyword));
            var data = _mapper.Map<List<ControllerActionDto>>(query);
            return data;
        }
        public async Task<bool> UpsertRolePermission(IList<ActionPermission> actionPermissions)
        {
            try
            {
                _context.ActionPermissions.AddRangeAsync(actionPermissions);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }
        public async Task<bool> CheckActionPermission(int roleId, string targetArea, string targetController)
        {
            var query = from ca in _context.ControllerActions
                        join ap in _context.ActionPermissions
                        on ca.Id equals ap.ControllerActionId into joinedData
                        from subap in joinedData.DefaultIfEmpty()
                        where subap.RoleId == roleId &&
                         ca.Area == targetArea &&
                        ca.Controller == targetController
                        select new ControllerActionDto
                        {
                            Pid = ca.Pid,
                            Id = ca.Id,
                            Controller = ca.Controller,
                            Action = ca.Action,
                            Area = ca.Area,
                            Method = ca.Method,
                            HasPermission = subap != null
                        };
            var data = _mapper.Map<List<ControllerActionDto>>(query);
            if(data != null && data.Count>0 )
                return true;
            else return false;
        }
    }
}
