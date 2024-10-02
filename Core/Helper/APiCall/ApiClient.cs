using Core.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace Core.Helper.APiCall
{
    public class ApiClient
    {
        public async Task<T> GetAsyncResult<T>(string apiPath, HttpMethod httpMethod, Dictionary<string, string> headerParam, Object? requestBody = null)
        {
            var result = string.Empty;
            HttpResponseMessage response = null;
            try
            {
                HttpClient _httpClient = new HttpClient();
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                if (headerParam.Count > 0)
                {
                    foreach (KeyValuePair<string, string> keyValue in headerParam)
                    {
                        _httpClient.DefaultRequestHeaders.Add(keyValue.Key, keyValue.Value);
                    }
                }
                _httpClient.BaseAddress = new Uri(apiPath);

                var request = new HttpRequestMessage(httpMethod, apiPath);
                string json = (requestBody == null) ? "" : JsonConvert.SerializeObject(requestBody);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");

                response = await _httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    result = response.Content.ReadAsStringAsync().Result;
                }
            }
            catch (Exception ex)
            {
                
            }
            return JsonConvert.DeserializeObject<T>(result);
        }
    }
}
