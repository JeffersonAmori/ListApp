using ListApp.Models;
using ListApp.Services.Interfaces;
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
    [QueryProperty(nameof(ShouldRefresh), nameof(ShouldRefresh))]
    public class HomeViewModel : BaseViewModel
    {
        private string _newListText;
        private List _selectedList;
        private bool _shouldRefresh;

        public ObservableCollection<List> ListCollection { get; }
        public IDataStore<List> DataStore => DependencyService.Get<IDataStore<List>>();
        public IDialogService DialogService => DependencyService.Get<IDialogService>();

        public ICommand LoadListsCommand { get; }
        public ICommand ListTappedCommand { get; }
        public ICommand AddListCommand { get; }
        public ICommand DeleteList { get; }

        public List SelectedList
        {
            get => _selectedList;
            set
            {
                SetProperty(ref _selectedList, value);
                OnPropertyChanged(nameof(SelectedList));
                OnListSelected(value);
            }
        }

        public string NewListText
        {
            get => _newListText;
            set
            {
                _newListText = value;
                OnPropertyChanged(nameof(NewListText));
            }
        }

        public bool ShouldRefresh
        {
            get => _shouldRefresh;
            set
            {
                _shouldRefresh = value;
                if (_shouldRefresh)
                    Task.Run(ExecuteLoadListsCommand);
            }
        }

        async private void OnListSelected(List list)
        {
            if (list == null)
                return;

            await Shell.Current.GoToAsync($"{nameof(ItemsPage)}?{nameof(ItemsViewModel.ListId)}={list.ListId}");
        }

        public HomeViewModel()
        {
            Title = "List Freak";

            ListCollection = new ObservableCollection<List>();

            LoadListsCommand = new Command(async () => await ExecuteLoadListsCommand());
            AddListCommand = new Command(async () => await AddToListCollection());
            DeleteList = new Command<string>(async (listId) => await OnDeleteList(listId));
            Task.Run(async () => await ExecuteLoadListsCommand());
        }

        private async Task OnDeleteList(string listId)
        {
            var currentList = ListCollection.FirstOrDefault(l => l.ListId == listId);

            if (currentList == null) return;

            bool deleteList = await DialogService.DisplayAlert($"Delete list {currentList.Name}?", "This action cannot be undone.", "Yes", "No");

            if (deleteList)
            {
                await DataStore.DeleteItemAsync(currentList.ListId);
                await Shell.Current.GoToAsync($"..?{nameof(HomeViewModel.ShouldRefresh)}={true}");
            }
        }

        private async Task AddToListCollection()
        {
            string newListName = await DialogService.DisplayPromptAsync("New list", String.Empty);

            if (string.IsNullOrEmpty(newListName))
                return;

            List list = new List()
            {
                ListId = Guid.NewGuid().ToString(),
                Name = newListName
            };

            await DataStore.AddItemAsync(list);

            ListCollection.Add(list);

            await Task.FromResult(true);
        }

        private async Task ExecuteLoadListsCommand()
        {
            IsBusy = true;

            try
            {
                ListCollection.Clear();
                var items = await DataStore.GetItemsAsync(true);
                foreach (var item in items)
                {
                    ListCollection.Add(item);
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
    }
}