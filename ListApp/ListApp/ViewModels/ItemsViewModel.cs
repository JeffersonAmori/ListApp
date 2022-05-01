using ListApp.Models;
using ListApp.Services;
using ListApp.Views;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
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
        public IDataStore<List> DataStore => DependencyService.Get<IDataStore<List>>();
        public ObservableCollection<ListItem> Items { get; }
        public ICommand LoadItemsCommand { get; }
        public ICommand AddItemCommand { get; }
        public ICommand DeleteItemCommand { get; }
        public Command<ListItem> ItemTapped { get; }
        public Command<object> CompletionItemButtonCommand { get; }

        public string ListId
        {
            get
            {
                return _listId;
            }
            set
            {
                _listId = value;
                Task.Run(ExecuteLoadItemsCommand);
                _currentList = DataStore.GetItemAsync(value).Result;
                Title = _currentList.Name;
            }
        }

        public ItemsViewModel()
        {
            Items = new ObservableCollection<ListItem>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            AddItemCommand = new Command(OnAddItem);
            CompletionItemButtonCommand = new Command<object>(OnCompletionButtonClicked);
            DeleteItemCommand = new Command<object>(OnDeleteItem);
        }

        async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            try
            {
                if (_currentList is null)
                    return;

                Items.Clear();
                foreach (var item in _currentList.ListItems)
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

        public void OnAppearing()
        {
            IsBusy = true;
            SelectedItem = null;
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

        private async void OnAddItem()
        {
            if (string.IsNullOrEmpty(NewItemText))
                return;

            ListItem listITem = new ListItem()
            {
                ListId = _currentList.ListId,
                Id = Guid.NewGuid().ToString(),
                Text = NewItemText
            };

            Items.Add(listITem);

            _currentList.ListItems.Add(listITem);
            await DataStore.UpdateItemAsync(_currentList);

            NewItemText = string.Empty;
        }

        private async void OnDeleteItem(object id)
        {
            ListItem listITem = _currentList.ListItems.First(x => x.Id == id.ToString());
            _currentList.ListItems.Remove(listITem);
            await DataStore.UpdateItemAsync(_currentList);
            Items.Remove(Items.First(i => i.Id == id.ToString()));
        }

        private async void OnCompletionButtonClicked(object Id)
        {
            var item = Items.FirstOrDefault(x => x.Id == Id.ToString());
            if (item != null)
            {
                // Move the item to the bottom of the list
                item.Checked = !item.Checked;
                Items.Remove(item);
                Items.Add(item);
            }

            await Task.FromResult(Task.CompletedTask);
        }
    }
}