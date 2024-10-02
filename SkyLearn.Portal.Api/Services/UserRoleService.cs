using Application.Models;
using Microsoft.EntityFrameworkCore;
using Application;
using Application.BaseManager;
using AutoMapper;

namespace SkyLearn.Portal.Api.Services
{
    public class UserRoleService : RestService
    {
        private readonly Context _context;
        private readonly IMapper _mapper;
        public UserRoleService(Context context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper=mapper;
        }
        public async Task<bool> UpsertUserRoles(IList<UserRole> userRoles)
        {
            if (userRoles.Count > 0)
            {
                string userIdx = userRoles[0].UserIdx.ToString();
                var existingUserRoles = await _context.UserRoles.Where(ur => ur.UserIdx == userIdx).ToListAsync();
                if (existingUserRoles != null && existingUserRoles.Count > 0)
                {
                    _context.UserRoles.RemoveRange(existingUserRoles);
                }
                foreach (var userRole in userRoles)
                {
                    userRole.UserIdx = userIdx;
                    _context.UserRoles.Add(userRole);
                }
                await _context.SaveChangesAsync();
                return true;

            }
            throw new AppException("Empty details");
        }
        public async Task<IList<UserRole>> GetUserRoles(string userIdx)
        {
            try
            {
                return await _context.UserRoles.Where(ur => ur.UserIdx == userIdx).ToListAsync();
            }
            catch(Exception ex)
            {
                throw ex;
            }

        }

        public async Task<List<RoleListDTO>> GetRoles(string userIdx)
        {
                try
                {
                    var query = from rls in _context.Roles
                                    join uls in _context.UserRoles
                                    on rls.Id equals uls.RoleId into joinedData
                                    from subap in joinedData.DefaultIfEmpty()
                                    where subap.UserIdx == userIdx
                                    select new RoleListDTO
                                    {
                                        Pid = rls.Pid,
                                        Name = rls.RoleName
                                    };

                    return await query.ToListAsync();
                }
                catch(Exception ex)
                {
                    throw ex;
                }

        }

        public async Task<bool> RemoveAllRoles(string userIDX)
        {
            try{
                await this._context.UserRoles.Where(usr=>usr.UserIdx==userIDX).ExecuteDeleteAsync();
                return true;
            }
            catch(Exception ex)
            {
                throw ex;
                
            }
 
        }
    }
}
