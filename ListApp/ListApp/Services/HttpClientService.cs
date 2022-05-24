using ListApp.Services.Interfaces;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ListApp.Services
{
    public class HttpClientService : IHttpClientService
    {
        private static HttpClient _httpClient = new HttpClient();

        public async Task<HttpResponseMessage> GetAsync(string requestUri)
        {
            return await _httpClient.GetAsync(requestUri);
        }

        public async Task<HttpResponseMessage> PutAsync(string requestUri, HttpContent content)
        {
            return await _httpClient.PutAsync(requestUri, content);
        }
        public async Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content)
        {
            return await _httpClient.PostAsync(requestUri, content);
        }

        public void SetBaseAddress(Uri baseAddress)
        {
            _httpClient.BaseAddress = baseAddress;
        }
    }
}
