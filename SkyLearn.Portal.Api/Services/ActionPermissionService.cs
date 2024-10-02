using Application.BaseManager;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace SkyLearn.Portal.Api.Services
{
    public class ActionPermissionService : RestService
    {
        private readonly Context _context;
        private readonly IMapper _mapper;
        public ActionPermissionService(Context context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
        }
    }
}
