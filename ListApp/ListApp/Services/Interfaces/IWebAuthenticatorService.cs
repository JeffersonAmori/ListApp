using System;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace ListApp.Services.Interfaces
{
    public interface IWebAuthenticatorService
    {
        Task<WebAuthenticatorResult> AuthenticateAsync(Uri url, Uri callbackUrl);
    }
}