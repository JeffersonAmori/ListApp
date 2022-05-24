using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ListApp.Services.Interfaces
{
    public interface IHttpClientService
    {
        Task<HttpResponseMessage> GetAsync(string requestUri);
        Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content);
        Task<HttpResponseMessage> PutAsync(string requestUri, HttpContent content);
        void SetBaseAddress(Uri baseAddress);
    }
}