using System.Threading.Tasks;
using Xamarin.Forms;

namespace ListApp.Services.Interfaces
{
    public interface IDialogService
    {
        public Task<string> DisplayPromptAsync(string title, string message, string accept = "OK", string cancel = "Cancel", string placeholder = null, int maxLength = -1, Keyboard keyboard = null, string initialValue = "");
        Task<bool> DisplayAlert(string title, string message, string accept, string cancel);
        Task DisplayAlert(string title, string message, string accept);
        Task DisplayToastAsync(string message);
    }
}
