using ListApp.Models;
using ListApp.Services.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ListApp.ViewModels
{
    [QueryProperty(nameof(ListId), nameof(ListId))]
    public class ItemsViewModel : BaseViewModel
    {
        private string _newItemText;
        private string _listId;
        private ListItem _selectedItem;
        private List _currentList;
        private readonly ILogger _logger;
        private readonly IDataStore<List> _dataStore;
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;
        private readonly IShareService _shareService;

        public ObservableCollection<ListItem> Items { get; }
        public ICommand LoadItemsCommand { get; }
        public ICommand AddItemCommand { get; }
        public ICommand DeleteItemCommand { get; }
        public ICommand DeleteListCommand { get; }
        public ICommand ShareListCommand { get; }
        public ICommand AddItemCompletedCommand { get; }
        public ICommand ItemDragAndDropFinishedCommand { get; }
        public ICommand ItemTapped { get; }
        public ICommand CompletionItemButtonCommand { get; }
        public ICommand CompletedListItemEntryCommand { get; }
        public ICommand RestoreListFromTrashBin { get; }

        public string ListId
        {
            get
            {
                return _listId;
            }
            set
            {
                _listId = value;
                CurrentList = _dataStore.GetItemAsync(value).Result;
                new Action(async () => await ExecuteLoadItemsCommand())();
                OnPropertyChanged(nameof(IsDeletedList));
                Title = CurrentList.Name;
            }
        }

        public bool IsDeletedList
        {
            get => CurrentList?.IsDeleted ?? false;
        }

        public ItemsViewModel(ILogger logger, IDataStore<List> dataStore, IDialogService dialogService, INavigationService navigationService, IShareService shareService)
        {
            Items = new ObservableCollection<ListItem>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            AddItemCommand = new Command(OnAddItem);
            CompletionItemButtonCommand = new Command<string>(OnCompletionButtonClicked);
            DeleteItemCommand = new Command<object>(OnDeleteItem);
            DeleteListCommand = new Command(OnDeleteList);
            AddItemCompletedCommand = new Command(OnAddItemCompletedCommand);
            ShareListCommand = new Command(OnShareListCommand);
            ItemDragAndDropFinishedCommand = new Command(OnItemDragAndDropFinishedCommand);
            CompletedListItemEntryCommand = new Command<ListItem>(OnCompletedListItemEntryCommand);
            RestoreListFromTrashBin = new Command(OnRestoreListFromTrashBin);
            _logger = logger;
            _dataStore = dataStore;
            _dialogService = dialogService;
            _navigationService = navigationService;
            _shareService = shareService;
        }

        public ListItem SelectedItem
        {
            get => _selectedItem;
            set
            {
                SetProperty(ref _selectedItem, value);
            }
        }

        public string NewItemText
        {
            get => _newItemText;
            set
            {
                _newItemText = value;
                OnPropertyChanged(nameof(NewItemText));
            }
        }

        public List CurrentList { get => _currentList; set => _currentList = value; }

        private async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            try
            {
                if (CurrentList is null) return;

                Items.Clear();
                foreach (var item in CurrentList.ListItems.OrderBy(li => li.Index))
                {
                    Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                _logger.TrackError(ex);
            }
            finally
            {
                IsBusy = false;
            }

            await Task.FromResult(Task.CompletedTask);
        }

        private void UpdateListItemsIndexes()
        {
            for (int i = 0; i < Items.Count; i++)
                Items[i].Index = i;
        }

        private async void OnAddItem()
        {
            if (string.IsNullOrEmpty(NewItemText))
                return;

            try
            {
                int nextIndex = Items.Any()
                                    ? Items.Max(i => i.Index) + 1
                                    : 1;

                ListItem listItem = new ListItem()
                {
                    ListId = CurrentList.ListId,
                    Id = Guid.NewGuid().ToString(),
                    Text = NewItemText,
                    Index = nextIndex
                };

                Items.Add(listItem);

                CurrentList.ListItems.Add(listItem);
                await _dataStore.UpdateItemAsync(CurrentList);

                NewItemText = string.Empty;
            }
            catch (Exception ex)
            {
                _logger.TrackError(ex);
            }
        }

        private async void OnDeleteItem(object id)
        {
            if (CurrentList.ListItems.FirstOrDefault(x => x.Id == id.ToString()) is ListItem listItem)
            {
                try
                {
                    CurrentList.ListItems.Remove(listItem);
                    await _dataStore.UpdateItemAsync(CurrentList);
                    Items.Remove(Items.First(i => i.Id == id.ToString()));
                    UpdateListItemsIndexes();
                }
                catch (Exception ex)
                {
                    _logger.TrackError(ex);
                }
            }
        }

        private async void OnDeleteList()
        {
            try
            {
                Task dialogTask = Task.FromResult(true);
                if (CurrentList.IsDeleted)
                {
                    bool deleteList = await _dialogService.DisplayAlert($"Delete list {CurrentList.Name}?", "This action cannot be undone.", "Yes", "No");

                    if (deleteList)
                    {
                        await _dataStore.DeleteItemAsync(CurrentList.ListId);
                        dialogTask = _dialogService.DisplayToastAsync("List deleted.");
                    }
                }
                else
                {
                    CurrentList.IsDeleted = true;
                    await _dataStore.UpdateItemAsync(CurrentList);
                    dialogTask = _dialogService.DisplayToastAsync("List moved to trash.");
                }

                await _navigationService.GoToAsync($"..?{nameof(ListViewModel.ShouldRefresh)}={true}");
                await dialogTask;
            }
            catch (Exception ex)
            {
                _logger.TrackError(ex);
            }
        }

        private async void OnCompletionButtonClicked(string Id)
        {
            try
            {
                var item = Items.FirstOrDefault(x => x.Id == Id);
                if (item != null)
                {
                    item.Checked = !item.Checked;
                    // Move the item to the bottom of the list
                    if (item.Checked)
                    {
                        Items.Remove(item);
                        Items.Add(item);
                    }

                    OnPropertyChanged(nameof(Items));

                    UpdateListItemsIndexes();
                    await _dataStore.UpdateItemAsync(CurrentList);
                }
            }
            catch (Exception ex)
            {
                _logger.TrackError(ex);
            }
        }

        private void OnAddItemCompletedCommand(object obj)
        {
            if (obj as Entry is Entry entry)
                entry.Focus();
        }

        private async void OnItemDragAndDropFinishedCommand()
        {
            try
            {
                UpdateListItemsIndexes();
                await _dataStore.UpdateItemAsync(CurrentList);
            }
            catch (Exception ex)
            {
                _logger.TrackError(ex);
            }
        }

        private async void OnShareListCommand(object obj)
        {
            try
            {
                if (Items.Count == 0)
                {
                    await _dialogService.DisplayAlert("Nothing to share", "The current list is empty.", "OK");
                    return;
                }

                StringBuilder listAsTextStringBuilder = new StringBuilder();
                listAsTextStringBuilder.Append(CurrentList.Name);
                foreach (var item in Items)
                {
                    listAsTextStringBuilder.Append($"\n - {item.Text}");
                }

                await _shareService.RequestAsync(listAsTextStringBuilder.ToString(), CurrentList.Name);
            }
            catch (Exception ex)
            {
                _logger.TrackError(ex);
            }
        }

        private async void OnCompletedListItemEntryCommand(ListItem listItem)
        {
            try
            {
                ListItem newListItem = new ListItem()
                {
                    ListId = CurrentList.ListId,
                    Id = Guid.NewGuid().ToString(),
                    Text = listItem.Text,
                    Checked = listItem.Checked
                };

                if (Items.Count == 1)
                    Items.Insert(0, newListItem);
                else
                    Items.Insert(listItem.Index, newListItem);

                listItem.Text = string.Empty;
                listItem.Checked = false;

                UpdateListItemsIndexes();

                CurrentList.ListItems.Add(newListItem);
                await _dataStore.UpdateItemAsync(CurrentList);
            }
            catch (Exception ex)
            {
                _logger.TrackError(ex);
            }
        }

        private async void OnRestoreListFromTrashBin()
        {
            try
            {
                CurrentList.IsDeleted = false;
                await _dataStore.UpdateItemAsync(CurrentList);
                await _navigationService.GoToAsync($"..?{nameof(ListViewModel.ShouldRefresh)}={true}");
            }
            catch (Exception ex)
            {
                _logger.TrackError(ex);
            }
        }
    }
}