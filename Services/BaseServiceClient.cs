using MageWin.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MageWin.Services
{
    public class BaseClientService : IBaseClientService
    {
        private readonly HttpClient _httpClient;

        public BaseClientService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<T> CallService<T>(string url, HttpMethod method, object requestBody = null, string token = null, object parameters = null)
        {
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            if (parameters != null)
            {
                url += "?" + ToQueryString(parameters);
            }

            HttpContent requestBodyContent = null;
            if (requestBody != null)
            {
                var jsonRequestBody = Newtonsoft.Json.JsonConvert.SerializeObject(requestBody);
                requestBodyContent = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");
            }

            var request = new HttpRequestMessage(method, url);
            if (method != HttpMethod.Get && requestBodyContent != null)
            {
                request.Content = requestBodyContent;
            }

            HttpResponseMessage response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(responseContent);
            }
            else
            {
                throw new HttpRequestException($"Erro na requisição: {response.StatusCode}");
            }
        }

        private string ToQueryString(object parameters)
        {
            var properties = parameters.GetType().GetProperties();
            var keyValuePairs = properties.Select(p => $"{p.Name}={HttpUtility.UrlEncode(p.GetValue(parameters)?.ToString() ?? "")}");
            return string.Join("&", keyValuePairs);
        }
    }
}
