using ListApp.Models;
using ListApp.Services.Interfaces;
using ListApp.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ListApp.ViewModels
{
    [QueryProperty(nameof(ShouldRefresh), nameof(ShouldRefresh))]
    public class ListViewModel : BaseViewModel
    {
        private string _newListText;
        private List _selectedList;
        private bool _shouldRefresh;
        private bool _isDeleted;
        private List<IListVisualItem> _listVisualItemCollection;
        private ILogger _logger;
        private IDataStore<List> _dataStore;
        private IDialogService _dialogService;
        private INavigationService _navigationService;

        public ObservableCollection<List> ListCollection { get; }
        public List<IListVisualItem> ListVisualItemCollection
        {
            get => _listVisualItemCollection;
            set
            {
                _listVisualItemCollection = value;
                OnPropertyChanged();
            }
        }
        public ICommand LoadListsCommand { get; }
        public ICommand ListTappedCommand { get; }
        public ICommand AddListCommand { get; }
        public ICommand ListDragAndDropFinishedCommand { get; }

        public List SelectedList
        {
            get => _selectedList;
            set
            {
                SetProperty(ref _selectedList, value);
                OnPropertyChanged(nameof(SelectedList));
                new Action(async () => await OnListSelected(value))();
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
                    new Action(async () => await ExecuteLoadListsCommand())();
            }
        }

        public bool IsDeleted
        {
            get { return _isDeleted; }
            set { _isDeleted = value; }
        }

        public ListViewModel(ILogger logger, IDataStore<List> dataStore, IDialogService dialogService, INavigationService navigationService)
        {
            Title = IsDeleted ? "Recycle bin" : "List Freak";

            ListCollection = new ObservableCollection<List>();
            ListVisualItemCollection = new List<IListVisualItem>();

            LoadListsCommand = new Command(async () => await ExecuteLoadListsCommand());
            ListTappedCommand = new Command<List>(async (list) => await OnListSelected(list));
            AddListCommand = new Command(async () => await AddToListCollection());
            ListDragAndDropFinishedCommand = new Command(async () => await OnListDragAndDropFinishedCommand());

            IsDeleted = (Shell.Current?.CurrentItem?.CurrentItem?.Route ?? string.Empty) == "IMPL_RecycleBin";

            _logger = logger;
            _dataStore = dataStore;
            _dialogService = dialogService;
            _navigationService = navigationService;
        }

        public void OnAppearing()
        {
            new Action(async () => await ExecuteLoadListsCommand())();
        }

        async private Task OnListSelected(List list)
        {
            try
            {
                if (list == null) return;

                await _navigationService.GoToAsync($"{nameof(ItemsPage)}?{nameof(ItemsViewModel.ListId)}={list.ListId}");
            }
            catch (Exception ex)
            {
                _logger.TrackError(ex);
            }
        }

        private async Task AddToListCollection()
        {
            try
            {
                string newListName = await _dialogService.DisplayPromptAsync("New list", String.Empty);

                if (string.IsNullOrEmpty(newListName))
                    return;

                int nextIndex = ListCollection.Any()
                                ? ListCollection.Max(l => l.Index) + 1
                                : 1;

                List list = new List()
                {
                    ListId = Guid.NewGuid().ToString(),
                    Name = newListName,
                    Index = nextIndex
                };

                await _dataStore.AddItemAsync(list);

                ListCollection.Add(list);
            }
            catch (Exception ex)
            {
                _logger.TrackError(ex);
            }
        }

        private async Task ExecuteLoadListsCommand()
        {
            IsBusy = true;

            try
            {
                ListCollection.Clear();
                var items = (await _dataStore.GetItemsAsync(true)).Where(list => list.IsDeleted == IsDeleted);
                foreach (var item in items.OrderBy(list => list.Index))
                {
                    ListCollection.Add(item);
                }

                var result = new List<IListVisualItem>() /*{ new ListHeader() }*/;

                foreach (var group in ListCollection.OrderBy(list => list.IsDeleted).GroupBy(list => list.IsDeleted))
                {
                    result.Add(new ListGroupHeader() { Name = group.Key ? "Deleted" : "Active" });
                    result.AddRange(group.Select(list => list));
                }

                //result.Add(new ListFooter());
                ListVisualItemCollection = result;
            }
            catch (Exception ex)
            {
                _logger.TrackError(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task OnListDragAndDropFinishedCommand()
        {
            try
            {
                UpdateListIndexes();
                foreach (var list in ListCollection)
                    await _dataStore.UpdateItemAsync(list);
            }
            catch (Exception ex)
            {
                _logger.TrackError(ex);
            }
        }

        private void UpdateListIndexes()
        {
            for (int i = 0; i < ListCollection.Count; i++)
                ListCollection[i].Index = i;
        }
    }
}