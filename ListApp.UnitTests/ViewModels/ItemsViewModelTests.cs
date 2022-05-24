using AutoFixture.NUnit3;
using FluentAssertions;
using ListApp.Services.Interfaces;
using ListApp.UnitTests.Base;
using ListApp.UnitTests.DataTtributes.Base;
using ListApp.ViewModels;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppModel = ListApp.Models;


namespace ListApp.UnitTests.ViewModels
{
    [TestFixture]
    public class ItemsViewModelTests : BaseTest
    {
        [Test, BaseAutoData]
        public async Task LoadItemsCommand_Should_PopulateItemsCollection(
            [Frozen] Mock<IDataStore<AppModel.ListItem>> mockIDataStore,
            ItemsViewModel sut)
        {
            // Arrange
            var listsThatShouldBeAdded = (await mockIDataStore.Object.GetItemsAsync()).Where(x => !x.IsDeleted);

            // Act
            sut.LoadItemsCommand.Execute(null);

            // Assert
            sut.Items.Should().BeEquivalentTo(sut.CurrentList.ListItems);
        }

        [Test, BaseAutoData]
        public void AddItemCommand_Should_DoNothing_When_NewItemTextIsBlank(
            ItemsViewModel sut)
        {
            // Arrange
            int originalAmoutOfLists = sut.Items.Count;
            sut.NewItemText = string.Empty;

            // Act
            sut.AddItemCommand.Execute(null);

            // Assert
            sut.Items.Count.Should().Be(originalAmoutOfLists);
        }

        [Test, BaseAutoData]
        public void AddItemCommand_Should_AddNewlyCreatedListItem(
            string listItemText,
            [Frozen] Mock<IDataStore<AppModel.List>> mockIDataStore,
            ItemsViewModel sut)
        {
            // Arrange
            int originalAmoutOfLists = sut.Items.Count;
            sut.NewItemText = listItemText;

            // Act
            sut.AddItemCommand.Execute(null);

            // Assert
            sut.Items.Should().Contain(x => x.Text == listItemText);
            mockIDataStore.Verify(x => x.UpdateItemAsync(It.Is<AppModel.List>((x) => x == sut.CurrentList)));
        }

        [Test, BaseAutoData]
        public void AddItemCommand_Should_LogException_OnError(
            string newItemText,
            [Frozen] Mock<ILogger> logger,
            [Frozen] Mock<IDataStore<AppModel.List>> mockIDataStore,
            ItemsViewModel sut)
        {
            // Arrange
            sut.NewItemText = newItemText;
            mockIDataStore
                .Setup(x => x.UpdateItemAsync(It.IsAny<AppModel.List>()))
                .Throws(new Exception($"Exception from test {nameof(AddItemCommand_Should_LogException_OnError)}"));

            // Act
            sut.AddItemCommand.Execute(null);

            // Assert
            logger.Verify(logger => logger.TrackError(It.IsAny<Exception>(), It.IsAny<Dictionary<string, string>>()), Times.Once);
        }

        [Test, BaseAutoData]
        public void CompletionItemButtonCommand_Should_MarkItemAsCompleted(
            ItemsViewModel sut)
        {
            // Arrange
            string itemId = sut.Items.First().Id;

            // Act
            sut.CompletionItemButtonCommand.Execute(itemId);

            // Assert
            var itemChecked = sut.Items.First(x => x.Id == itemId);
            itemChecked.Checked.Should().BeTrue();
            itemChecked.Index.Should().Be(sut.Items.IndexOf(itemChecked));
        }

        [Test, BaseAutoData]
        public void CompletionItemButtonCommand_Should_LorException_OnError(
            [Frozen] Mock<ILogger> logger,
            [Frozen] Mock<IDataStore<AppModel.List>> mockIDataStore,
            ItemsViewModel sut)
        {
            // Arrange
            var item = sut.Items.First();
            item.Checked = false;
            mockIDataStore
                .Setup(x => x.UpdateItemAsync(It.IsAny<AppModel.List>()))
                .Throws(new Exception($"Exception from test {nameof(CompletionItemButtonCommand_Should_LorException_OnError)}"));

            // Act
            sut.CompletionItemButtonCommand.Execute(item.Id);

            // Assert
            logger.Verify(logger => logger.TrackError(It.IsAny<Exception>(), It.IsAny<Dictionary<string, string>>()), Times.Once);
        }

        [Test, BaseAutoData]
        public void DeleteItemCommand_Sould_DeleteSelectedItem_FromCurrentList(ItemsViewModel sut)
        {
            // Arrange
            var itemToDelete = sut.CurrentList.ListItems.First();

            // Act
            sut.DeleteItemCommand.Execute(itemToDelete.Id);

            // Assert
            sut.CurrentList.ListItems.Should().NotContain(itemToDelete);
        }

        [Test, BaseAutoData]
        public void DeleteItemCommand_Sould_DeleteSelectedItem_FromItemsCollection(ItemsViewModel sut)
        {
            // Arrange
            var itemToDelete = sut.CurrentList.ListItems.First();
            sut.Items.Clear();
            foreach (var item in sut.CurrentList.ListItems)
                sut.Items.Add(item);

            // Act
            sut.DeleteItemCommand.Execute(itemToDelete.Id);

            // Assert
            sut.Items.Should().NotContain(itemToDelete);
        }

        [Test, BaseAutoData]
        public void DeleteItemCommand_Sould_DeleteSelectedItem_FromDatabase(
            [Frozen] Mock<IDataStore<AppModel.List>> mockIDataStore,
            ItemsViewModel sut)
        {
            // Arrange
            var itemToDelete = sut.CurrentList.ListItems.First();

            // Act
            sut.DeleteItemCommand.Execute(itemToDelete.Id);

            // Assert
            mockIDataStore.Verify(x => x.UpdateItemAsync(It.Is<AppModel.List>(x => x == sut.CurrentList)));
        }

        [Test, BaseAutoData]
        public void DeleteItemCommand_Sould_ReindexItems_After_DeleteSelectedItem(ItemsViewModel sut)
        {
            // Arrange
            var itemToDelete = sut.CurrentList.ListItems.First();
            sut.Items.Clear();
            foreach (var item in sut.CurrentList.ListItems)
                sut.Items.Add(item);

            // Act
            sut.DeleteItemCommand.Execute(itemToDelete.Id);

            // Assert
            foreach (var item in sut.Items)
                item.Index.Should().Be(sut.Items.IndexOf(item));
        }

        [Test, BaseAutoData]
        public void DeleteItemCommand_Sould_LogException_OnError(
            string itemId,
            [Frozen] Mock<ILogger> logger,
            [Frozen] Mock<IDataStore<AppModel.List>> mockIDataStore,
            ItemsViewModel sut)
        {
            // Arrange
            var itemToDelete = sut.CurrentList.ListItems.First();
            mockIDataStore
                .Setup(x => x.UpdateItemAsync(It.IsAny<AppModel.List>()))
                .Throws(new Exception($"Exception from test {nameof(DeleteItemCommand_Sould_LogException_OnError)}"));

            // Act
            sut.DeleteItemCommand.Execute(itemToDelete.Id);

            // Assert
            logger.Verify(logger => logger.TrackError(It.IsAny<Exception>(), It.IsAny<Dictionary<string, string>>()), Times.Once);
        }

        [Test, BaseAutoData]
        public void DeleteListCommand_Should_MarkListAsDeleted_When_ItIsNotFlaggedAsDeleted(ItemsViewModel sut)
        {
            // Arrange
            sut.CurrentList.IsDeleted = false;

            // Act
            sut.DeleteListCommand.Execute(null);

            // Assert
            sut.CurrentList.IsDeleted.Should().BeTrue();
        }

        [Test, BaseAutoData]
        public void DeleteListCommand_Should_UpdateTheListOnTheDatabase_When_ItIsNotFlaggedAsDeleted(
            [Frozen] Mock<IDataStore<AppModel.List>> mockIDataStore,
            ItemsViewModel sut)
        {
            // Arrange
            sut.CurrentList.IsDeleted = false;

            // Act
            sut.DeleteListCommand.Execute(null);

            // Assert
            mockIDataStore.Verify(x => x.UpdateItemAsync(It.Is<AppModel.List>(x => x == sut.CurrentList)));
        }

        [Test, BaseAutoData]
        public void DeleteListCommand_Should_NotifyUser_When_ItIsNotFlaggedAsDeleted(
            [Frozen] Mock<IDialogService> dialogService,
            ItemsViewModel sut)
        {
            // Arrange
            sut.CurrentList.IsDeleted = false;

            // Act
            sut.DeleteListCommand.Execute(null);

            // Assert
            dialogService.Verify(x => x.DisplayToastAsync(It.IsAny<string>()));
        }

        [Test, BaseAutoData]
        public void DeleteListCommand_Should_PermanentlyDeleteList_When_ItIsFlaggedAsDeleted(
            [Frozen] Mock<IDialogService> dialogService,
            [Frozen] Mock<IDataStore<AppModel.List>> mockIDataStore,
            ItemsViewModel sut)
        {
            // Arrange
            sut.CurrentList.IsDeleted = true;
            dialogService
                .Setup(x => x.DisplayAlert(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            // Act
            sut.DeleteListCommand.Execute(null);

            // Assert
            mockIDataStore.Verify(x => x.DeleteItemAsync(It.Is<string>(x => x == sut.CurrentList.ListId)));
        }

        [Test, BaseAutoData]
        public void DeleteListCommand_Should_NotifyUserOfDeletion_When_ItIsFlaggedAsDeleted(
            [Frozen] Mock<IDialogService> dialogService,
            ItemsViewModel sut)
        {
            // Arrange
            sut.CurrentList.IsDeleted = true;
            dialogService
                .Setup(x => x.DisplayAlert(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            // Act
            sut.DeleteListCommand.Execute(null);

            // Assert
            dialogService.Verify(x => x.DisplayToastAsync(It.IsAny<string>()), Times.Once);
        }

        [Test, BaseAutoData]
        public void DeleteListCommand_Should_NavigateBack_AfterDeletion(
            [Frozen] Mock<INavigationService> navigationService,
            [Frozen] Mock<IDialogService> dialogService,
            ItemsViewModel sut)
        {
            // Arrange
            sut.CurrentList.IsDeleted = true;
            dialogService
                .Setup(x => x.DisplayAlert(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            // Act
            sut.DeleteListCommand.Execute(null);

            // Assert
            navigationService.Verify(x => x.GoToAsync($"..?{nameof(ListViewModel.ShouldRefresh)}={true}", It.IsAny<bool>()), Times.Once);
        }

        [Test, BaseAutoData]
        public void DeleteListCommand_Should_LorException_OnError(
            [Frozen] Mock<ILogger> logger,
            [Frozen] Mock<IDataStore<AppModel.List>> mockIDataStore,
            ItemsViewModel sut)
        {
            // Arrange
            sut.CurrentList.IsDeleted = false;
            mockIDataStore
                .Setup(x => x.UpdateItemAsync(It.IsAny<AppModel.List>()))
                .Throws(new Exception($"Exception from test {nameof(CompletionItemButtonCommand_Should_LorException_OnError)}"));

            // Act
            sut.DeleteListCommand.Execute(null);

            // Assert
            logger.Verify(logger => logger.TrackError(It.IsAny<Exception>(), It.IsAny<Dictionary<string, string>>()), Times.Once);
        }

        [Test, BaseAutoData]
        public void ItemDragAndDropFinishedCommand_Should_ReindexItems(ItemsViewModel sut)
        {
            // Act
            sut.ItemDragAndDropFinishedCommand.Execute(null);

            // Assert
            foreach (var item in sut.Items)
                item.Index.Should().Be(sut.Items.IndexOf(item));
        }

        [Test, BaseAutoData]
        public void ItemDragAndDropFinishedCommand_Should_UpdateListOnDatabase(
            [Frozen] Mock<ILogger> logger,
            [Frozen] Mock<IDataStore<AppModel.List>> mockIDataStore,
            ItemsViewModel sut)
        {
            // Arrange
            mockIDataStore
                .Setup(x => x.UpdateItemAsync(It.IsAny<AppModel.List>()))
                .Throws(new Exception($"Exception from test {nameof(ItemDragAndDropFinishedCommand_Should_UpdateListOnDatabase)}"));

            // Act
            sut.ItemDragAndDropFinishedCommand.Execute(null);

            // Assert
            logger.Verify(logger => logger.TrackError(It.IsAny<Exception>(), It.IsAny<Dictionary<string, string>>()), Times.Once);
        }


        [Test, BaseAutoData]
        public void ShareListCommand_Should_NotifyUserThereIsNothingToShare_When_ItemsCollectionIsEmpty(
            [Frozen] Mock<IDialogService> dialogService,
            ItemsViewModel sut)
        {
            // Arrange
            sut.Items.Clear();

            // Act
            sut.ShareListCommand.Execute(null);

            // Assert
            dialogService.Verify(x => x.DisplayAlert(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }


        [Test, BaseAutoData]
        public void ShareListCommand_Should_ShareList(
            [Frozen] Mock<IShareService> shareService,
            ItemsViewModel sut)
        {
            // Act
            sut.ShareListCommand.Execute(null);

            // Assert
            shareService.Verify(x => x.RequestAsync(It.Is<string>(
                // Checks if the shared text contains the text from all itmes as well the name of the list
                s => s.Contains(sut.CurrentList.Name) &&
                     sut.Items.Select(x => s.Contains(x.Text)).All(b => b)),
                sut.CurrentList.Name), Times.Once);
        }

        [Test, BaseAutoData]
        public void ItemDragAndDropFinishedCommand_Should_LogException_OnError(
            [Frozen] Mock<ILogger> logger,
            [Frozen] Mock<IShareService> shareService,
            ItemsViewModel sut)
        {
            // Arrange
            shareService
                .Setup(x => x.RequestAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new Exception($"Exception from test {nameof(ItemDragAndDropFinishedCommand_Should_LogException_OnError)}"));

            // Act
            sut.ShareListCommand.Execute(null);

            // Assert
            logger.Verify(logger => logger.TrackError(It.IsAny<Exception>(), It.IsAny<Dictionary<string, string>>()), Times.Once);
        }

        [Test]
        [BaseInlineAutoData(1)]
        [BaseInlineAutoData(2)]
        [BaseInlineAutoData(3)]
        public void CompletedListItemEntryCommand_Should_ReIndexAllItems(int index, ItemsViewModel sut)
        {
            // Arrange
            var itemCompleted = sut.Items.First();
            itemCompleted.Index = index;

            // Act
            sut.CompletedListItemEntryCommand.Execute(itemCompleted);

            // Assert
            foreach (var item in sut.Items)
                item.Index.Should().Be(sut.Items.IndexOf(item));
        }


        [Test, BaseAutoData]
        public void CompletedListItemEntryCommand_Should_AddNewlyCreatedItemToTheDatabase(
            [Frozen] Mock<IDataStore<AppModel.List>> dataStore,
            ItemsViewModel sut)
        {
            // Arrange
            var itemCompleted = sut.Items.First();
            itemCompleted.Index = 1;

            // Act
            sut.CompletedListItemEntryCommand.Execute(itemCompleted);

            // Assert
            dataStore.Verify(x => x.UpdateItemAsync(sut.CurrentList));
        }

        [Test, BaseAutoData]
        public void CompletedListItemEntryCommand_Should_LogException_OnError(
            [Frozen] Mock<ILogger> logger,
            [Frozen] Mock<IDataStore<AppModel.List>> mockIDataStore,
            ItemsViewModel sut)
        {
            // Arrange
            var itemCompleted = sut.Items.First();
            mockIDataStore
                .Setup(x => x.UpdateItemAsync(It.IsAny<AppModel.List>()))
                .Throws(new Exception($"Exception from test {nameof(CompletedListItemEntryCommand_Should_LogException_OnError)}"));

            // Act
            sut.CompletedListItemEntryCommand.Execute(itemCompleted);

            // Assert
            logger.Verify(logger => logger.TrackError(It.IsAny<Exception>(), It.IsAny<Dictionary<string, string>>()), Times.Once);
        }

        [Test, BaseAutoData]
        public void RestoreListFromTrashBinCommand_Should_UpdateListOnTheDatabase(
            [Frozen] Mock<IDataStore<AppModel.List>> dataStore,
            ItemsViewModel sut)
        {
            // Arrange
            var itemCompleted = sut.Items.First();
            itemCompleted.Index = 1;

            // Act
            sut.RestoreListFromTrashBin.Execute(itemCompleted);

            // Assert
            sut.CurrentList.IsDeleted.Should().BeFalse();
            dataStore.Verify(x => x.UpdateItemAsync(sut.CurrentList));
        }

        [Test, BaseAutoData]
        public void RestoreListFromTrashBinCommand_Should_ShouldNavigateBack_After(
            [Frozen] Mock<INavigationService> navigationService,
            ItemsViewModel sut)
        {
            // Arrange
            var itemCompleted = sut.Items.First();
            itemCompleted.Index = 1;

            // Act
            sut.RestoreListFromTrashBin.Execute(itemCompleted);

            // Assert
            sut.CurrentList.IsDeleted.Should().BeFalse();
            navigationService.Verify(x => x.GoToAsync(It.Is<string>(x => x.StartsWith("..")), It.IsAny<bool>()), Times.Once);
        }

        [Test, BaseAutoData]
        public void RestoreListFromTrashBinCommand_Should_LogException_OnError(
            [Frozen] Mock<ILogger> logger,
            [Frozen] Mock<IDataStore<AppModel.List>> mockIDataStore,
            ItemsViewModel sut)
        {
            // Arrange
            var itemCompleted = sut.Items.First();
            mockIDataStore
                .Setup(x => x.UpdateItemAsync(It.IsAny<AppModel.List>()))
                .Throws(new Exception($"Exception from test {nameof(RestoreListFromTrashBinCommand_Should_LogException_OnError)}"));

            // Act
            sut.RestoreListFromTrashBin.Execute(itemCompleted);

            // Assert
            logger.Verify(logger => logger.TrackError(It.IsAny<Exception>(), It.IsAny<Dictionary<string, string>>()), Times.Once);
        }
    }
}
