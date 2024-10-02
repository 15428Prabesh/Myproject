using Application;
using Application.BaseManager;
using AutoMapper;

namespace SkyLearn.Portal.Api.Services
{
    public class FormDataService : RestService
    {
        private readonly Context _context;
        public FormDataService(Context context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
        }
    }
}
