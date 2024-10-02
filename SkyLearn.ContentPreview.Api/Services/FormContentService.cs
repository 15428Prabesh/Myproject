using Application;
using Application.BaseManager;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace SkyLearn.ContentPreview.Api.Services
{
    public class FormContentService : RestService
    {
        private readonly ContentContextPreview _context;
        public FormContentService(ContentContextPreview context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
        }

        public virtual async Task<T> RetrieveByName<T>(string Name) where T : class
        {

            var contents = await _context.Set<T>()
                .Where(x => EF.Property<bool>(x, "IsDeleted") == false && EF.Property<string>(x, "Title") == Name)
                .FirstOrDefaultAsync();
            return contents;
        }
    }
}
