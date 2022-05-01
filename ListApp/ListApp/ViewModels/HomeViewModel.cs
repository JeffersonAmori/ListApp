using ListApp.Models;
using ListApp.Services;
using ListApp.Views;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ListApp.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        private string _newListText;
        private List _selectedList;
        public ObservableCollection<List> ListCollection { get; }
        public IDataStore<List> DataStore => DependencyService.Get<IDataStore<List>>();
        public ICommand LoadListsCommand { get; }
        public ICommand ListTappedCommand { get; }
        public ICommand AddListCommand { get; }

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

        async private void OnListSelected(List list)
        {
            if (list == null)
                return;

            SelectedList = null;
            await Shell.Current.GoToAsync($"{nameof(ItemsPage)}?{nameof(ItemsViewModel.ListId)}={list.ListId}");
        }

        public HomeViewModel()
        {
            Title = "Mobile notes";

            ListCollection = new ObservableCollection<List>();

            LoadListsCommand = new Command(async () => await ExecuteLoadListsCommand());
            AddListCommand = new Command(async () => await AddToListCollection());
            Task.Run(async () => await ExecuteLoadListsCommand());
        }

        private async Task AddToListCollection()
        {
            string newListName = await Application.Current.MainPage.DisplayPromptAsync("New list", "New list");

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