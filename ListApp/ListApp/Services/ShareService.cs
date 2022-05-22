using ListApp.Services.Interfaces;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace ListApp.Services
{
    public class ShareService : IShareService
    {
        public async Task RequestAsync(string text, string title)
        {
            await Share.RequestAsync(text, title);
        }
    }
}
