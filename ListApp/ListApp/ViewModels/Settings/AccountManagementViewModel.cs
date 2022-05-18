using ListApp.Helpers;
using ListApp.Models;
using ListApp.Models.Extensions;
using ListApp.Resources;
using ListApp.Services.Interfaces;
using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using ApiModel = ListApp.Api.Models;

namespace ListApp.ViewModels.Settings
{
    public class AccountManagementViewModel : BaseViewModel
    {
        private ILogger _logger = DependencyService.Get<ILogger>();
        private IDataStore<List> _dataStore = DependencyService.Get<IDataStore<List>>();
        private IDialogService _dialogService => DependencyService.Get<IDialogService>();
        public ICommand LoginWithGoogleCommand { get; }
        public ICommand SyncCommand { get; }
        public ICommand SignOutCommand { get; }
        private bool _isSyncing;


        public bool IsSyncing
        {
            get { return _isSyncing; }
            set
            {
                _isSyncing = value;
                OnPropertyChanged();
            }
        }

        public AccountManagementViewModel()
        {
            Title = "Account";
            LoginWithGoogleCommand = new Command(OnLoginWithGoogleCommand);
            SyncCommand = new Command(OnSyncCommand);
            SignOutCommand = new Command(OnSignOutCommand);
        }

        private async void OnLoginWithGoogleCommand()
        {
            try
            {
                var authResult = await WebAuthenticator.AuthenticateAsync(
                    new Uri(Secrets.ListFreakAuthServerEndpoint.ToString() + "mobileauth/Google"),
                    new Uri("listfreak://"));

                var fullName = WebUtility.UrlDecode(authResult.Properties["name"]);
                var email = WebUtility.UrlDecode(authResult.Properties["email"]);
                var accessToken = authResult?.AccessToken;
                var refreshToken = authResult?.RefreshToken;

                ApplicationUser.Current.Set(fullName, email, accessToken, refreshToken);
                Preferences.Set(PreferencesKeys.ApplicationUserInfo, JsonSerializer.Serialize(ApplicationUser.Current));
            }
            catch (Exception ex)
            {
                _logger.TrackError(ex);
            }
        }

        private async void OnSyncCommand()
        {
            IsSyncing = true;

            try
            {
                var syncStartedDialogTask = _dialogService.DisplayToastAsync("Syncing...");

                HttpClient httpClient = new HttpClient();
                httpClient.BaseAddress = Secrets.ListFreakApiEndpoint;

                // Start 2 tasks: 1) to retrieve data from local DB and 2) to request the online lists to the API
                var allBackedupListsForCurrentUserRequestTask = httpClient.GetAsync($"lists/ownerEmail/{ApplicationUser.Current.Email}");
                var localListsTask = _dataStore.GetItemsAsync();

                var localLists = await localListsTask;
                var allBackedupListsForCurrentUserRequest = await allBackedupListsForCurrentUserRequestTask;
                var allBackedupListsForCurrentUser = await JsonSerializer.DeserializeAsync<List<ApiModel.List>>(await allBackedupListsForCurrentUserRequest.Content.ReadAsStreamAsync());

                foreach (var cloudList in allBackedupListsForCurrentUser)
                {
                    if (localLists.FirstOrDefault(x => x.ListId == cloudList.Guid) is List localList)
                    {
                        // Update the cloud list based on local data.
                        cloudList.CopyFrom(localList);
                        cloudList.LastChangedDate = DateTime.UtcNow;

                        string jsonContent = JsonSerializer.Serialize(cloudList);
                        var response = await httpClient.PutAsync($"lists/{cloudList.Id}", new StringContent(jsonContent, Encoding.UTF8, "application/json"));
                    }
                    else
                    {
                        // Insert the list from the cloud into the local DB.
                        await _dataStore.AddItemAsync(cloudList.ToLocalModel());
                    }
                }

                foreach (var localList in localLists)
                {
                    if (!allBackedupListsForCurrentUser.Any(x => x.Guid == localList.ListId))
                    {
                        var cloudList = localList.ToApiModel();
                        var response = await httpClient.PostAsync($"lists", new StringContent(JsonSerializer.Serialize(cloudList), Encoding.UTF8, "application/json"));
                    }
                }

                var syncFinishedDialogTask = _dialogService.DisplayToastAsync("Synced!");
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
            finally
            {
                IsSyncing = false;
            }
        }

        private async void OnSignOutCommand()
        {
            try
            {
                ApplicationUser.Current.Unset();
                Preferences.Remove(PreferencesKeys.ApplicationUserInfo);
                await _dialogService.DisplayToastAsync("Signed out");
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }
    }
}
