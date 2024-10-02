using Microsoft.EntityFrameworkCore;
using Application;
using Application.BaseManager;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using NuGet.Protocol;

namespace SkyLearn.Portal.Api.Services
{
    public class SemesterService : RestService
    {
        private readonly Context _context;
        public SemesterService(Context context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
        }

        public override Task<T> Retrieve<T>(string Pid)
        {
            _context.Semesters
                .Include(dep => dep.Department).Include(dep=>dep.Department).ToList();

            return base.Retrieve<T>(Pid);
        }


    }
}
