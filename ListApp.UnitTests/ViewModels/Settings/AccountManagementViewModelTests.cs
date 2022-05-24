using AutoFixture.NUnit3;
using ListApp.Resources;
using ListApp.Services.Interfaces;
using ListApp.UnitTests.DataTtributes;
using ListApp.ViewModels.Settings;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using AppModel = ListApp.Models;
using ApiModel = ListApp.Api.Models;
using ListApp.Models.Extensions;
using FluentAssertions;
using ListApp.UnitTests.Base;
using ListApp.UnitTests.DataTtributes.Base;
using Xamarin.Essentials;
using System.Net;
using ListApp.Models;

namespace ListApp.UnitTests.ViewModels.Settings
{
    [TestFixture]
    public class AccountManagementViewModelTests : BaseTest
    {
        [Test, BaseAutoData]
        public void SyncCommand_Should_AddToDatabase_MissingLists(
            [Frozen] Mock<IHttpClientService> httpClientService,
            [Frozen] Mock<IDataStore<AppModel.List>> dataStore,
            List<ApiModel.List> lists,
            AccountManagementViewModel sut)
        {
            // Arrange
            httpClientService
                .Setup(x => x.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(new HttpResponseMessage()
                {
                    Content = JsonContent.Create(lists)
                });

            // Act
            sut.SyncCommand.Execute(null);

            // Assert
            dataStore.Verify(
                d => d.AddItemAsync(
                    It.Is<AppModel.List>(
                        x => lists.Any(y => y.Guid == x.ListId))),
                Times.Exactly(3));
        }

        [Test, BaseAutoData]
        public void SyncCommand_Should_UpdateCloudLists_BasedOn_LocalLists(
            [Frozen] Mock<IHttpClientService> httpClientService,
            [Frozen] Mock<IDataStore<AppModel.List>> dataStore,
            List<AppModel.List> localLists,
            List<ApiModel.List> cloudLists,
            AccountManagementViewModel sut)
        {
            // Arrange
            for (int i = 0; i < localLists.Count; i++)
                localLists[i].ListId = cloudLists[i].Guid = Guid.NewGuid().ToString();

            httpClientService
                .Setup(x => x.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(new HttpResponseMessage()
                {
                    Content = JsonContent.Create(cloudLists)
                });

            dataStore
                .Setup(x => x.GetItemsAsync(It.IsAny<bool>()))
                .ReturnsAsync(localLists);

            // Act
            sut.SyncCommand.Execute(null);

            // Assert
            httpClientService
                .Verify(
                    x => x.PutAsync(
                        It.IsAny<string>(), It.IsAny<HttpContent>()),
                    Times.Exactly(localLists.Count));
        }

        [Test, BaseAutoData]
        public void SyncCommand_Should_LogException_OnError(
            [Frozen] Mock<IDataStore<AppModel.List>> dataStore,
            [Frozen] Mock<ILogger> logger,
            AccountManagementViewModel sut)
        {
            // Arrange
            dataStore
                .Setup(x => x.GetItemsAsync(It.IsAny<bool>()))
                .Throws(new Exception($"Exception from test {nameof(SyncCommand_Should_LogException_OnError)}"));

            // Act
            sut.SyncCommand.Execute(null);

            // Assert
            logger.Verify(x => x.TrackError(It.IsAny<Exception>(), It.IsAny<Dictionary<string, string>>()), Times.Once);
        }

        [Test, BaseAutoData]
        public void SyncCommand_Should_NotifyUser_OnError(
            [Frozen] Mock<IDataStore<AppModel.List>> dataStore,
            [Frozen] Mock<IDialogService> dialogService,
            AccountManagementViewModel sut)
        {
            // Arrange
            dataStore
                .Setup(x => x.GetItemsAsync(It.IsAny<bool>()))
                .Throws(new Exception($"Exception from test {nameof(SyncCommand_Should_NotifyUser_OnError)}"));

            // Act
            sut.SyncCommand.Execute(null);

            // Assert
            dialogService.Verify(x => x.DisplayToastAsync(It.Is<string>(x => x == "Something went wrong.")), Times.Once);
        }

        [Test, BaseAutoData]
        public void LoginWithGoogleCommand_CurrentApplicationUser_Should_BeSetWithValuesReturnedFromAuth(
            string name,
            string email,
            WebAuthenticatorResult webAuthenticatorResult,
            [Frozen] Mock<IWebAuthenticatorService> webAuthenticatorService,
            AccountManagementViewModel sut)
        {
            // Arrange
            webAuthenticatorResult.Properties["name"] = WebUtility.UrlEncode(name);
            webAuthenticatorResult.Properties["email"] = WebUtility.UrlEncode(email);
            webAuthenticatorService
                .Setup(x => x.AuthenticateAsync(It.IsAny<Uri>(), It.IsAny<Uri>()))
                .ReturnsAsync(webAuthenticatorResult);

            // Act
            sut.LoginWithGoogleCommand.Execute(null);

            // Assert
            ApplicationUser.Current.FullName.Should().Be(name);
            ApplicationUser.Current.Email.Should().Be(email);
            ApplicationUser.Current.AcessToken.Should().Be(webAuthenticatorResult.AccessToken);
            ApplicationUser.Current.RefreshToken.Should().Be(webAuthenticatorResult.RefreshToken);
            ApplicationUser.Current.IsLoggedIn.Should().BeTrue();
        }

        [Test, BaseAutoData]
        public void LoginWithGoogleCommand_Should_LogException_OnError(
            [Frozen] Mock<IWebAuthenticatorService> webAuthenticatorService,
            [Frozen] Mock<ILogger> logger,
            AccountManagementViewModel sut)
        {
            // Arrange
            webAuthenticatorService
                .Setup(x => x.AuthenticateAsync(It.IsAny<Uri>(), It.IsAny<Uri>()))
                .Throws(new Exception($"Exception from test {nameof(LoginWithGoogleCommand_Should_LogException_OnError)}"));

            // Act
            sut.LoginWithGoogleCommand.Execute(null);

            // Assert
            logger.Verify(x => x.TrackError(It.IsAny<Exception>(), It.IsAny<Dictionary<string, string>>()), Times.Once);
        }

        [Test, BaseAutoData]
        public void SignOutCommand_Should_UnsetCurrentApplicationUser(
            ApplicationUser appUser,
            AccountManagementViewModel sut)
        {
            // Arrange
            ApplicationUser.Current.Set(appUser);

            // Act
            sut.SignOutCommand.Execute(null);

            // Assert
            ApplicationUser.Current.FirstName.Should().BeEmpty();
            ApplicationUser.Current.Email.Should().BeEmpty();
            ApplicationUser.Current.AcessToken.Should().BeEmpty();
            ApplicationUser.Current.RefreshToken.Should().BeEmpty();
            ApplicationUser.Current.IsLoggedIn.Should().BeFalse();
        }

        [Test, BaseAutoData]
        public void SignOutCommand_Should_RemoveUserInfor_FromPreferences(
            [Frozen] Mock<IPreferencesService> preferencesService,
            AccountManagementViewModel sut)
        {
            // Act
            sut.SignOutCommand.Execute(null);

            // Assert
            preferencesService.Verify(x => x.Remove(It.Is<string>(s => s == PreferencesKeys.ApplicationUserInfo)));
        }

        [Test, BaseAutoData]
        public void SignOutCommand_Should_LogException_OnError(
            [Frozen] Mock<IPreferencesService> preferencesService,
            [Frozen] Mock<ILogger> logger,
            AccountManagementViewModel sut)
        {
            // Arrange
            preferencesService
                .Setup(x => x.Remove(It.IsAny<string>()))
                .Throws(new Exception($"Exception from test {nameof(SignOutCommand_Should_LogException_OnError)}"));

            // Act
            sut.SignOutCommand.Execute(null);

            // Assert
            logger.Verify(x => x.TrackError(It.IsAny<Exception>(), It.IsAny<Dictionary<string, string>>()), Times.Once);
        }
    }
}

