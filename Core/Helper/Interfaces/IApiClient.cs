using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Helper.Interfaces
{
    public interface IApiClient
    {
        Task<T> PostAsync<T>(object jsonContent, string apiPath);
        Task<T> PostAsync<T>(object jsonContent, string apiPath, bool addClientHeader = false, bool accessToken = false);
        Task<T> PostAsync<T>(object jsonContent, string apiPath, Dictionary<string, string> headerParam, bool addClientHeader = false, bool accessToken = false);


        Task<T> GetAsync<T>();
        Task<T> GetAsync<T>(string apiPath);
        Task<T> GetAsync<T>(string apiPath, bool addClientHeader = false, bool accessToken = false);
        Task<T> GetAsync<T>(string apiPath, Dictionary<string, string> headerParam, bool addClientHeader = false, bool accessToken = false);

        Task PostVoidAsync(object jsonContent, string apiPath);

    }
}
