using Application.BaseManager;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace SkyLearn.Portal.Api.Services
{
    public class ComponentService:RestService
    {
        private readonly Context _context;
        public ComponentService(Context context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
        }

        public virtual async Task<T> RetrieveByName<T>(string Name) where T : class
        {

            var contents = await _context.Set<T>()
                .Where(x => EF.Property<bool>(x, "IsDeleted") == false && EF.Property<string>(x, "Name") == Name)
                .FirstOrDefaultAsync();
            return contents;
        }
    }
}
