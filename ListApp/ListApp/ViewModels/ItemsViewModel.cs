using ListApp.Models;
using ListApp.Services;
using ListApp.Views;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ListApp.ViewModels
{
    [QueryProperty(nameof(ListId), nameof(ListId))]
    public class ItemsViewModel : BaseViewModel
    {
        private string _newItemText;
        private string _listId;
        private ListItem _selectedItem;

        public IDataStore<ListItem> DataStore => DependencyService.Get<IDataStore<ListItem>>();
        public ObservableCollection<ListItem> Items { get; }
        public Command LoadItemsCommand { get; }
        public Command AddItemCommand { get; }
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
            }
        }

        public ItemsViewModel()
        {
            Title = "Browse";
            Items = new ObservableCollection<ListItem>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            ItemTapped = new Command<ListItem>(OnItemSelected);

            AddItemCommand = new Command(OnAddItem);

            CompletionItemButtonCommand = new Command<object>(OnCompletionButtonClicked);
        }

        async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            try
            {
                Items.Clear();
                var items = await DataStore.GetItemsAsync(true);
                foreach (var item in items)
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
                OnItemSelected(value);
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

            Items.Add(new ListItem()
            {
                Id = Guid.NewGuid().ToString(),
                Text = NewItemText
            });

            NewItemText = string.Empty;

            await Task.FromResult(true);
        }

        async void OnItemSelected(ListItem item)
        {
            if (item == null)
                return;

            // This will push the ItemDetailPage onto the navigation stack
            await Shell.Current.GoToAsync($"{nameof(ItemDetailPage)}?{nameof(ItemDetailViewModel.ItemId)}={item.Id}");
        }

        private async void OnCompletionButtonClicked(object Id)
        {
            var item = Items.FirstOrDefault(x => x.Id == Id.ToString());
            if (item != null)
            {
                // Move the item to the bottom of the list
                Items.Remove(item);
                Items.Add(item);
            }
        }
    }
}