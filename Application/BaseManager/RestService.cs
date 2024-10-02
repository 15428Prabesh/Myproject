using Application.Helpers;
using Application.Models;
using AutoMapper;
using Core;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Application.BaseManager
{
    public class RestService : IService
    {
        private readonly DbContext _context;
        private IMapper _mapper;

        public RestService(DbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public virtual async Task<T> Create<T>(T model, string uniqueKey = "", string uniqueValue = "") where T : class
        {
            if (!string.IsNullOrEmpty(uniqueKey) && !string.IsNullOrEmpty(uniqueValue))
            {
                var contents = await _context.Set<T>()
                    .Where(x => EF.Property<bool>(x, "IsDeleted") == false &&
                        (EF.Property<string>(x, uniqueKey) == uniqueValue))
                        .ToListAsync();
                if (contents.Count > 0)
                    return model;
            }

            await _context.Set<T>().AddAsync(model);
            _context.SaveChanges();
            return model;
        }

        public virtual async Task<T> Update<T>(string Pid, JsonPatchDocument<T> model) where T : class
        {
            var entity = await Retrieve<T>(Pid);
            model.ApplyTo(entity);
            await _context.SaveChangesAsync(true);
            return entity;
        }
        public virtual async Task<T> UpdatePut<T>(string Pid, T model) where T : class
        {
            var entity = await Retrieve<T>(Pid);
            if (entity != null)
            {
                _context.Entry(entity).CurrentValues.SetValues(model);
                await _context.SaveChangesAsync(true);
                return entity;
            }
            throw new Exception($"Entity with ID {Pid} not found.");
        }
        public virtual async Task<bool> Delete<T>(T model, bool force_delete = false) where T : class
        {
            if (force_delete)
            {
                _context.Set<T>().Remove(model);
                await _context.SaveChangesAsync(true);
                return true;
            }
            else
            {
                var entity = _context.Entry(model);
                if (entity != null)
                {
                    entity.Property("IsDeleted").CurrentValue = true;
                    entity.Property("IsActive").CurrentValue = false;
                    entity.State = EntityState.Modified;
                }
                await _context.SaveChangesAsync(true);
                return true;
            }

        }

        public virtual async Task<T> Retrieve<T>(string Pid) where T : class
        {

            var contents = await _context.Set<T>()
                .Where(x => EF.Property<bool>(x, "IsDeleted") == false && EF.Property<string>(x, "Pid") == Pid)
                .FirstOrDefaultAsync();
            return contents;
        }

        public virtual async Task<T> RetrieveByID<T>(int id) where T : class
        {
            var contents = await _context.Set<T>().FindAsync(id);
            return contents;
        }

        public virtual async Task<object> List<T1, T2>(bool paginate = false, int pageSize = 10, int pageNumber = 0, string filterKey = "", string filterValue = "", string sortColumn = null, bool sortAsc = false) where T1 : class where T2 : class
        {
            var contents = await _context.Set<T1>().Where(x => EF.Property<bool>(x, "IsDeleted") == false).ToListAsync();
            if (!string.IsNullOrEmpty(sortColumn))

                if (sortAsc)
                {
                    contents = await _context.Set<T1>()
                       .Where(x => EF.Property<bool>(x, "IsDeleted") == false &&
                        (string.IsNullOrEmpty(filterKey) || EF.Property<string>(x, filterKey).Contains(filterValue)))
                        .OrderBy(x => EF.Property<string>(x, sortColumn))
                        .ToListAsync();
                }
                else
                {
                    contents = await _context.Set<T1>()
                       .Where(x => EF.Property<bool>(x, "IsDeleted") == false &&
                        (string.IsNullOrEmpty(filterKey) || EF.Property<string>(x, filterKey).Contains(filterValue)))
                        .OrderByDescending(x => EF.Property<string>(x, sortColumn))
                        .ToListAsync();
                }
            else
                contents = await _context.Set<T1>()
                          .Where(x => EF.Property<bool>(x, "IsDeleted") == false &&
                           (string.IsNullOrEmpty(filterKey) || EF.Property<string>(x, filterKey).Contains(filterValue)))
                           .OrderByDescending(x => EF.Property<DateTime>(x, "CreatedAt"))
                           .ToListAsync();


            if (paginate)
            {
                int totalCount = contents.Count;
                int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
                var result = contents
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
                var paginated_res = new PaginatedResult<T2>
                {
                    Data = _mapper.Map<IList<T2>>(result),
                    TotalCount = totalCount,
                    TotalPages = totalPages
                };
                return paginated_res;
            }
            var res = _mapper.Map<IList<T2>>(contents);
            return res;

        }


    }
}

