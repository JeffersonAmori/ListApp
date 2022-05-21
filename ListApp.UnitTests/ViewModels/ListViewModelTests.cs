using AutoFixture.NUnit3;
using FluentAssertions;
using ListApp.Services.Interfaces;
using ListApp.UnitTests.DataTtributes;
using ListApp.ViewModels;
using ListApp.Views;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using AppModel = ListApp.Models;

namespace ListApp.UnitTests.ViewModels
{
    [TestFixture]
    public class ListViewModelTests
    {
        [Test, BaseAutoData]
        public async Task LoadListsCommand_Should_PopulateListCollection(
            [Frozen] Mock<IDataStore<AppModel.List>> mockIDataStore,
            ListViewModel sut)
        {
            // Arrange
            sut.IsDeleted = false;
            var listsThatShouldBeAdded = (await mockIDataStore.Object.GetItemsAsync()).Where(x => !x.IsDeleted);

            // Act
            sut.LoadListsCommand.Execute(null);

            // Assert
            sut.ListCollection.Should().BeEquivalentTo(listsThatShouldBeAdded);
        }

        [Test, BaseAutoData]
        public void LoadListsCommand_Should_LogException_OnError(
            [Frozen] Mock<ILogger> logger,
            [Frozen] Mock<IDataStore<AppModel.List>> mockIDataStore,
            ListViewModel sut)
        {
            // Arrange
            mockIDataStore.Setup(x => x.GetItemsAsync(It.IsAny<bool>()))
                .Throws(new Exception($"Exception from test {nameof(LoadListsCommand_Should_LogException_OnError)}"));

            // Act
            sut.LoadListsCommand.Execute(null);

            // Assert
            logger.Verify(logger => logger.TrackError(It.IsAny<Exception>(), It.IsAny < Dictionary<string, string>>()), Times.Once);
        }

        [Test, BaseAutoData]
        public void ListTappedCommand_Should_DoNothingWhenInputIsNull(
            [Frozen] Mock<INavigationService> navigationService,
            ListViewModel sut)
        {
            // Act
            sut.ListTappedCommand.Execute(null);

            // Assert
            navigationService.Verify(x => x.GoToAsync(string.Empty, false), Times.Never);
        }

        [Test, BaseAutoData]
        public void ListTappedCommand_Should_NavigateToItemsPage(
            [Frozen] Mock<INavigationService> navigationService,
            ListViewModel sut)
        {
            // Act
            sut.ListTappedCommand.Execute(sut.SelectedList);

            // Assert
            string navigationString = $"{nameof(ItemsPage)}?{nameof(ItemsViewModel.ListId)}={sut.SelectedList.ListId}";
            navigationService.Verify(x => x.GoToAsync(navigationString, false));
        }

        [Test, BaseAutoData]
        public void ListTapped_Should_LogException_OnError(
            string newListName,
            [Frozen] Mock<ILogger> logger,
            [Frozen] Mock<INavigationService> navigationService,
            ListViewModel sut)
        {
            // Arrange
            navigationService
                .Setup(x => x.GoToAsync(It.IsAny<string>(), It.IsAny<bool>()))
                .Throws(new Exception($"Exception from test {nameof(ListTapped_Should_LogException_OnError)}"));


            // Act
            sut.ListTappedCommand.Execute(sut.SelectedList);

            // Assert
            logger.Verify(logger => logger.TrackError(It.IsAny<Exception>(), It.IsAny<Dictionary<string, string>>()), Times.Once);
        }

        [Test, BaseAutoData]
        public void AddListCommand_Should_DhNothing_When_UserLeavesListNameBlank(
            [Frozen] Mock<IDialogService> dialogService,
            ListViewModel sut)
        {
            // Arrange
            dialogService
                .Setup(d => d.DisplayPromptAsync(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<int>(), It.IsAny<Keyboard>(), It.IsAny<string>()))
                .ReturnsAsync(string.Empty);

            int originalAmoutOfLists = sut.ListCollection.Count;

            // Act
            sut.AddListCommand.Execute(sut.SelectedList);

            // Assert
            sut.ListCollection.Count.Should().Be(originalAmoutOfLists);
        }

        [Test, BaseAutoData]
        public void AddListCommand_Should_AddNewlyCreatedList(
            string newListName,
            [Frozen] Mock<IDialogService> dialogService,
            [Frozen] Mock<IDataStore<AppModel.List>> dataStore,
            ListViewModel sut)
        {
            // Arrange
            dialogService
                .Setup(d => d.DisplayPromptAsync(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<int>(), It.IsAny<Keyboard>(), It.IsAny<string>()))
                .ReturnsAsync(newListName);

            // Act
            sut.AddListCommand.Execute(null);

            // Assert
            dataStore.Verify(x => x.AddItemAsync(It.IsAny<AppModel.List>()));
            sut.ListCollection.Should().ContainSingle(x => x.Name == newListName);
        }

        [Test, BaseAutoData]
        public void AddListCommand_Should_LogException_OnError(
            string newListName,
            [Frozen] Mock<ILogger> logger,
            [Frozen] Mock<IDialogService> dialogService,
            [Frozen] Mock<IDataStore<AppModel.List>> mockIDataStore,
            ListViewModel sut)
        {
            // Arrange
            dialogService
                .Setup(d => d.DisplayPromptAsync(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<int>(), It.IsAny<Keyboard>(), It.IsAny<string>()))
                .ReturnsAsync(newListName);

            mockIDataStore.Setup(x => x.AddItemAsync(It.IsAny<AppModel.List>()))
                .Throws(new Exception($"Exception from test {nameof(AddListCommand_Should_LogException_OnError)}"));

            // Act
            sut.AddListCommand.Execute(null);

            // Assert
            logger.Verify(logger => logger.TrackError(It.IsAny<Exception>(), It.IsAny<Dictionary<string, string>>()), Times.Once);
        }

        [Test, BaseAutoData]
        public void OnListDragAndDropFinishedCommand_Should_ReorderLists(
            IEnumerable<AppModel.List> testData,
            [Frozen] Mock<IDataStore<AppModel.List>> dataStore,
            ListViewModel sut)
        {
            // Arrange
            foreach (var item in testData)
                sut.ListCollection.Add(item);

            // Act
            sut.ListDragAndDropFinishedCommand.Execute(null);

            // Assert
            dataStore.Verify(x => x.UpdateItemAsync(It.IsAny<AppModel.List>()), Times.Exactly(sut.ListCollection.Count()));
            foreach (var list in sut.ListCollection)
                list.Index.Should().Be(sut.ListCollection.IndexOf(list));
        }

        [Test, BaseAutoData]
        public void ListDragAndDropFinishedCommand_Should_LogException_OnError(
            IEnumerable<AppModel.List> testData,
            [Frozen] Mock<ILogger> logger,
            [Frozen] Mock<IDataStore<AppModel.List>> mockIDataStore,
            ListViewModel sut)
        {
            // Arrange
            foreach (var item in testData)
                sut.ListCollection.Add(item);

            mockIDataStore.Setup(x => x.UpdateItemAsync(It.IsAny<AppModel.List>()))
                .Throws(new Exception($"Exception from test {nameof(ListDragAndDropFinishedCommand_Should_LogException_OnError)}"));

            // Act
            sut.ListDragAndDropFinishedCommand.Execute(null);

            // Assert
            logger.Verify(logger => logger.TrackError(It.IsAny<Exception>(), It.IsAny<Dictionary<string, string>>()), Times.Once);
        }

        [Test, BaseAutoData]
        public void OnAppering_ShouldNot_Throw_AnyException(ListViewModel sut)
        {
            // Act
            sut.OnAppearing();

            Assert.Pass();
        }
    }
}
