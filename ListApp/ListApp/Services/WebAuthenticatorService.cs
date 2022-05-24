using ListApp.Services.Interfaces;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace ListApp.Services
{
    public class WebAuthenticatorService : IWebAuthenticatorService
    {
        public async Task<WebAuthenticatorResult> AuthenticateAsync(Uri url, Uri callbackUrl) =>
            await WebAuthenticator.AuthenticateAsync(url, callbackUrl);
    }
}
