using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;

namespace Application.Helpers
{
    public interface IService
    {
        public Task<T> Create<T>(T model, string uniqueKey = "", string uniqueValue = "") where T : class;
        public Task<T> Update<T>(string Pid, JsonPatchDocument<T> model) where T : class;
        public Task<bool> Delete<T>(T model, bool force_delete) where T : class;
        public Task<T> Retrieve<T>(string Pid) where T : class;
        public Task<T> RetrieveByID<T>(int id) where T : class;
        public Task<object> List<T1, T2>(bool paginate = false, int pageSize = 10, int pageNumber = 1, string filterKey = "", string filterValue = "", string sortColumn = null, bool sortAsc = false) where T1 : class where T2 : class;

    }
}
