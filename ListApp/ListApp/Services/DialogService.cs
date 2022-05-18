using ListApp.Services.Interfaces;
using System;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Forms;

namespace ListApp.Services
{
    public class DialogService : IDialogService
    {
        private ILogger _logger;

        public DialogService(ILogger logger)
        {
            _logger = logger;
        }

        public async Task<bool> DisplayAlert(string title, string message, string accept, string cancel)
        {
            try
            {
                return await Application.Current.MainPage.DisplayAlert(title, message, accept, cancel);
            }
            catch (Exception ex)
            {
                _logger.TrackError(ex);
                return false;
            }
        }

        public async Task DisplayAlert(string title, string message, string accept)
        {
            try
            {
                await Application.Current.MainPage.DisplayAlert(title, message, accept);
            }
            catch (Exception ex)
            {
                _logger.TrackError(ex);
            }
        }

        public async Task<string> DisplayPromptAsync(string title, string message, string accept = "OK", string cancel = "Cancel", string placeholder = null, int maxLength = -1, Keyboard keyboard = null, string initialValue = "")
        {
            try
            {
                return await Application.Current.MainPage.DisplayPromptAsync(title, message, accept, cancel, placeholder, maxLength, keyboard, initialValue);
            }
            catch (Exception ex)
            {
                _logger.TrackError(ex);
                return String.Empty;
            }
        }

        public async Task DisplayToastAsync(string message)
        {
            try
            {

                await Application.Current.MainPage.DisplayToastAsync(message);
            }
            catch (Exception ex)
            {
                _logger.TrackError(ex);
            }
        }
    }
}
