using ListApp.Models;
using ListApp.Services.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
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
        public IDataStore<List> DataStore => DependencyService.Get<IDataStore<List>>();
        public IDialogService DialogService => DependencyService.Get<IDialogService>();
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
                _currentList = DataStore.GetItemAsync(value).Result;
                Task.Run(async () => await ExecuteLoadItemsCommand());
                OnPropertyChanged(nameof(IsDeletedList));
                Title = _currentList.Name;
            }
        }

        public bool IsDeletedList
        {
            get => _currentList?.IsDeleted ?? false;
        }

        public ItemsViewModel()
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

        private async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            try
            {
                if (_currentList is null) return;

                Items.Clear();
                foreach (var item in _currentList.ListItems.OrderBy(li => li.Index))
                {
                    Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
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

            //for (int i = 0; i < _currentList.ListItems.Count; i++)
            //    _currentList.ListItems[i].Index = i;
        }

        private async void OnAddItem()
        {
            if (string.IsNullOrEmpty(NewItemText))
                return;

            int nextIndex = Items.Any()
                            ? Items.Max(i => i.Index) + 1
                            : 1;

            ListItem listItem = new ListItem()
            {
                ListId = _currentList.ListId,
                Id = Guid.NewGuid().ToString(),
                Text = NewItemText,
                Index = nextIndex
            };

            Items.Add(listItem);

            _currentList.ListItems.Add(listItem);
            await DataStore.UpdateItemAsync(_currentList);

            NewItemText = string.Empty;
        }

        private async void OnDeleteItem(object id)
        {
            ListItem listITem = _currentList.ListItems.FirstOrDefault(x => x.Id == id.ToString());

            if (listITem == null) return;

            _currentList.ListItems.Remove(listITem);
            await DataStore.UpdateItemAsync(_currentList);
            Items.Remove(Items.First(i => i.Id == id.ToString()));
            UpdateListItemsIndexes();
        }

        private async void OnDeleteList()
        {
            Task dialogTask = Task.FromResult(true);
            if (_currentList.IsDeleted)
            {
                bool deleteList = await DialogService.DisplayAlert($"Delete list {_currentList.Name}?", "This action cannot be undone.", "Yes", "No");

                if (deleteList)
                {
                    await DataStore.DeleteItemAsync(_currentList.ListId);
                    dialogTask =DialogService.DisplayToastAsync("List deleted.");
                }
            }
            else
            {
                _currentList.IsDeleted = true;
                await DataStore.UpdateItemAsync(_currentList);
                dialogTask = DialogService.DisplayToastAsync("List moved to trash.");
            }

            await Shell.Current.GoToAsync($"..?{nameof(ListViewModel.ShouldRefresh)}={true}");
            await dialogTask;
        }

        private async void OnCompletionButtonClicked(string Id)
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
                await DataStore.UpdateItemAsync(_currentList);
            }
        }

        private void OnAddItemCompletedCommand(object obj)
        {
            Entry entry = obj as Entry;

            if (entry == null) return;

            entry.Focus();
        }

        private async void OnItemDragAndDropFinishedCommand()
        {
            UpdateListItemsIndexes();
            await DataStore.UpdateItemAsync(_currentList);
        }

        private async void OnShareListCommand(object obj)
        {
            if (Items.Count == 0)
            {
                await DialogService.DisplayAlert("Nothing to share", "The current list is empty.", "OK");
                return;
            }

            StringBuilder listAsTextStringBuilder = new StringBuilder();
            listAsTextStringBuilder.Append(_currentList.Name);
            foreach (var item in Items)
            {
                listAsTextStringBuilder.Append($"\n - {item.Text}");
            }

            await Share.RequestAsync(listAsTextStringBuilder.ToString(), _currentList.Name);
        }

        private async void OnCompletedListItemEntryCommand(ListItem listItem)
        {
            ListItem newListItem = new ListItem()
            {
                ListId = _currentList.ListId,
                Id = Guid.NewGuid().ToString(),
                Text = listItem.Text
            };

            if (Items.Count == listItem.Index)
                Items.Add(newListItem);
            else
                Items.Insert(listItem.Index, newListItem);

            listItem.Text = string.Empty;

            UpdateListItemsIndexes();

            _currentList.ListItems.Add(newListItem);
            await DataStore.UpdateItemAsync(_currentList);
        }

        private async void OnRestoreListFromTrashBin()
        {
            _currentList.IsDeleted = false;
            await DataStore.UpdateItemAsync(_currentList);
            await Shell.Current.GoToAsync($"..?{nameof(ListViewModel.ShouldRefresh)}={true}");
        }
    }
}