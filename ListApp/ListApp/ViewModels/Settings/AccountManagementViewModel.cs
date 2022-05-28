using ListApp.Helpers;
using ListApp.Models;
using ListApp.Models.Extensions;
using ListApp.Resources;
using ListApp.Resources.Internationalization;
using ListApp.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Windows.Input;
using Xamarin.Forms;
using ApiModel = ListApp.Api.Models;

namespace ListApp.ViewModels.Settings
{
    public class AccountManagementViewModel : BaseViewModel
    {
        private readonly ILogger _logger;
        private readonly IDataStore<List> _dataStore;
        private readonly IDialogService _dialogService;
        private readonly IHttpClientService _httpClientService;
        private readonly IWebAuthenticatorService _webAuthenticatorService;
        private readonly IPreferencesService _preferencesService;
        private bool _isSyncing;

        public ICommand LoginWithGoogleCommand { get; }
        public ICommand SyncCommand { get; }
        public ICommand SignOutCommand { get; }

        public bool IsSyncing
        {
            get { return _isSyncing; }
            set
            {
                _isSyncing = value;
                OnPropertyChanged();
            }
        }

        public AccountManagementViewModel(
            IDataStore<List> dataStore,
            IDialogService dialogService,
            ILogger logger,
            IHttpClientService httpClientService,
            IWebAuthenticatorService webAuthenticatorService,
            IPreferencesService preferencesService)
        {
            LoginWithGoogleCommand = new Command(OnLoginWithGoogleCommand);
            SyncCommand = new Command(OnSyncCommand);
            SignOutCommand = new Command(OnSignOutCommand);

            _dataStore = dataStore;
            _dialogService = dialogService;
            _logger = logger;
            _httpClientService = httpClientService;
            _webAuthenticatorService = webAuthenticatorService;
            _preferencesService = preferencesService;

            _httpClientService.SetBaseAddress(Secrets.ListFreakApiEndpoint);
        }

        private async void OnLoginWithGoogleCommand()
        {
            try
            {
                var authResult = await _webAuthenticatorService.AuthenticateAsync(
                    new Uri(Secrets.ListFreakAuthServerEndpoint.ToString() + "mobileauth/Google"),
                    new Uri("listfreak://"));

                var fullName = WebUtility.UrlDecode(authResult.Properties["name"]);
                var email = WebUtility.UrlDecode(authResult.Properties["email"]);
                var accessToken = authResult?.AccessToken;
                var refreshToken = authResult?.RefreshToken;

                ApplicationUser.Current.Set(fullName, email, accessToken, refreshToken);
                _preferencesService.Set(PreferencesKeys.ApplicationUserInfo, JsonSerializer.Serialize(ApplicationUser.Current));
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
                _ = _dialogService.DisplayToastAsync(LocalizedResources.PageAccountSyncStarted);

                // Start 2 tasks: 1) to retrieve data from local DB and 2) to request the online lists to the API.
                var allBackedupListsForCurrentUserRequestTask = _httpClientService.GetAsync(string.Format(ListApiEndPoints.GetListsByOwnerEmail, ApplicationUser.Current.Email));
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
                        var response = await _httpClientService.PutAsync(string.Format(ListApiEndPoints.PutList, cloudList.Id), new StringContent(jsonContent, Encoding.UTF8, "application/json"));
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
                        var response = await _httpClientService.PostAsync(ListApiEndPoints.PostList, new StringContent(JsonSerializer.Serialize(cloudList), Encoding.UTF8, "application/json"));
                    }
                }

                _ = _dialogService.DisplayToastAsync(LocalizedResources.PageAccountSyncSuccessful);
            }
            catch (Exception ex)
            {
                _ = _dialogService.DisplayToastAsync(LocalizedResources.PageAccountSyncOnError);
                _logger.TrackError(ex);
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
                _preferencesService.Remove(PreferencesKeys.ApplicationUserInfo);
                await _dialogService.DisplayToastAsync(LocalizedResources.PageAccountSignedOut);
            }
            catch (Exception ex)
            {
                _logger.TrackError(ex);
            }
        }
    }
}
