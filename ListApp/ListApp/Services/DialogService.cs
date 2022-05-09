using ListApp.Services.Interfaces;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ListApp.Services
{
    public class DialogService : IDialogService
    {
        public async Task<bool> DisplayAlert(string title, string message, string accept, string cancel)
        {
            return await Application.Current.MainPage.DisplayAlert(title, message, accept, cancel);
        }

        public async Task DisplayAlert(string title, string message, string accept)
        {
            await Application.Current.MainPage.DisplayAlert(title, message, accept);
        }

        public async Task<string> DisplayPromptAsync(string title, string message, string accept = "OK", string cancel = "Cancel", string placeholder = null, int maxLength = -1, Keyboard keyboard = null, string initialValue = "")
        {
            return await Application.Current.MainPage.DisplayPromptAsync(title, message, accept, cancel, placeholder, maxLength, keyboard, initialValue);
        }
    }
}
