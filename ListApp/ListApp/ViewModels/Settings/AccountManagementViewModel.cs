using ListApp.Helpers;
using ListApp.Models;
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
        private IDataStore<List> DataStore = DependencyService.Get<IDataStore<List>>();
        public IDialogService DialogService => DependencyService.Get<IDialogService>();
        public ICommand LoginWithGoogleCommand { get; }
        public ICommand SyncCommand { get; }
        public ICommand SignOutCommand { get; }

        private string _token;

        public string Token
        {
            get { return _token; }
            set
            {
                _token = value;
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
            try
            {
                var syncStartedDialogTask = DialogService.DisplayToastAsync("Syncing...");

                HttpClient httpClient = new HttpClient();
                httpClient.BaseAddress = Secrets.ListFreakApiEndpoint;

                var allBackedupListsForCurrentUserRequest = await httpClient.GetAsync($"lists/ownerEmail/{ApplicationUser.Current.Email}");
                var allBackedupListsForCurrentUser = await JsonSerializer.DeserializeAsync<List<ApiModel.List>>(await allBackedupListsForCurrentUserRequest.Content.ReadAsStreamAsync());

                var localLists = await DataStore.GetItemsAsync();

                foreach (var cloudList in allBackedupListsForCurrentUser)
                {
                    if (localLists.FirstOrDefault(x => x.ListId == cloudList.Guid) is List localList)
                    {
                        // Update the list on the cloud.
                        cloudList.Name = localList.Name;
                        cloudList.IsDeleted = localList.IsDeleted;
                        cloudList.Index = localList.Index;
                        cloudList.LastChangedDate = DateTime.UtcNow;

                        foreach (var localListItem in localList.ListItems)
                        {
                            if (cloudList.ListItems.FirstOrDefault(x => x.Guid == localListItem.Id) is ApiModel.ListItem cloudListItem)
                            {
                                cloudListItem.IsDeleted = localListItem.IsDeleted;
                                cloudListItem.Text = localListItem.Text;
                                cloudListItem.Checked = localListItem.Checked;
                                cloudListItem.Description = localListItem.Description;
                                cloudListItem.Index = localListItem.Index;
                                cloudListItem.LastChangedDate = DateTime.UtcNow;
                            }
                            else
                            {
                                var listItem = new ApiModel.ListItem();
                                listItem.ListId = cloudList.Id;
                                listItem.Guid = localListItem.Id;
                                listItem.Text = localListItem.Text;
                                listItem.Description = localListItem.Description;
                                listItem.Index = localListItem.Index;
                                listItem.IsDeleted = localListItem.IsDeleted;
                                listItem.Checked = localListItem.Checked;
                                listItem.CreationDate = localListItem.CreationDate;
                                listItem.LastChangedDate = DateTime.UtcNow;
                                cloudList.ListItems.Add(listItem);
                            }
                        }

                        string jsonContent = JsonSerializer.Serialize(cloudList);
                        var response = await httpClient.PutAsync($"lists/{cloudList.Id}", new StringContent(jsonContent, Encoding.UTF8, "application/json"));
                    }
                    else
                    {
                        // Insert the list from the cloud into the local DB.
                        var list = new List();
                        list.ListId = cloudList.Guid;
                        list.Name = cloudList.Name;
                        list.IsDeleted = cloudList.IsDeleted;
                        list.Index = cloudList.Index;
                        list.CreationDate = cloudList.CreationDate;
                        list.LastChangedDate = DateTime.UtcNow;

                        foreach (var cloudListItem in cloudList.ListItems)
                        {
                            var localListItem = new ListItem();
                            localListItem.Id = cloudListItem.Guid;
                            localListItem.Text = cloudListItem.Text;
                            localListItem.Description = cloudListItem.Description;
                            localListItem.Index = cloudListItem.Index;
                            localListItem.IsDeleted = cloudListItem.IsDeleted;
                            localListItem.Checked = cloudListItem.Checked;
                            localListItem.CreationDate = cloudListItem.CreationDate;
                            localListItem.LastChangedDate = DateTime.UtcNow;
                            list.ListItems.Add(localListItem);
                        }

                        await DataStore.AddItemAsync(list);
                    }
                }

                foreach (var localList in localLists)
                {
                    if (!allBackedupListsForCurrentUser.Any(x => x.Guid == localList.ListId))
                    {
                        var cloudList = new ApiModel.List();
                        cloudList.Guid = localList.ListId;
                        cloudList.Name = localList.Name;
                        cloudList.IsDeleted = localList.IsDeleted;
                        cloudList.Index = localList.Index;
                        cloudList.OwnerEmail = ApplicationUser.Current.Email;
                        cloudList.CreationDate = localList.CreationDate;
                        cloudList.LastChangedDate = DateTime.UtcNow;

                        foreach (var localListItem in localList.ListItems)
                        {
                            var cloudListItem = new ApiModel.ListItem();
                            cloudListItem.Guid = localListItem.Id;
                            cloudListItem.Text = localListItem.Text;
                            cloudListItem.Description = localListItem.Description;
                            cloudListItem.Index = localListItem.Index;
                            cloudListItem.IsDeleted = localListItem.IsDeleted;
                            cloudListItem.Checked = localListItem.Checked;
                            cloudListItem.CreationDate = localListItem.CreationDate;
                            cloudListItem.LastChangedDate = DateTime.UtcNow;
                        }

                        var response = await httpClient.PostAsync($"lists", new StringContent(JsonSerializer.Serialize(cloudList), Encoding.UTF8, "application/json"));
                    }
                }

                var syncFinishedDialogTask = DialogService.DisplayToastAsync("Synced!");

                await Task.WhenAll(syncStartedDialogTask, syncFinishedDialogTask);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private async void OnSignOutCommand()
        {
            try
            {
                ApplicationUser.Current.Unset();
                Preferences.Remove(PreferencesKeys.ApplicationUserInfo);
                await DialogService.DisplayToastAsync("Signed out");
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }
    }
}
