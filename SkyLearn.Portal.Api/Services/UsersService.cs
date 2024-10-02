using Application;
using Application.BaseManager;
using AutoMapper;

namespace SkyLearn.Portal.Api.Services
{
    public class UsersService : RestService
    {
        private readonly Context _context;
        public UsersService(Context context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
        }
    }
}
