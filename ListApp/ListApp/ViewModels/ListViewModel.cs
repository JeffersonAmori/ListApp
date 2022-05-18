using ListApp.Models;
using ListApp.Services.Interfaces;
using ListApp.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
        private ILogger _logger = DependencyService.Get<ILogger>();
        public IDataStore<List> DataStore => DependencyService.Get<IDataStore<List>>();
        public IDialogService DialogService => DependencyService.Get<IDialogService>();

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
        public ICommand DeleteList { get; }
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

        public ListViewModel()
        {
            ListCollection = new ObservableCollection<List>();
            ListVisualItemCollection = new List<IListVisualItem>();

            LoadListsCommand = new Command(async () => await ExecuteLoadListsCommand());
            ListTappedCommand = new Command<List>(async (list) => await OnListSelected(list));
            AddListCommand = new Command(async () => await AddToListCollection());
            DeleteList = new Command<string>(async (listId) => await OnDeleteList(listId));
            ListDragAndDropFinishedCommand = new Command(async () => await OnListDragAndDropFinishedCommand());

            IsDeleted = Shell.Current.CurrentItem.CurrentItem.Route == "IMPL_RecycleBin";
            Title = IsDeleted ? "Recycle bin" : "List Freak";
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

                await Shell.Current.GoToAsync($"{nameof(ItemsPage)}?{nameof(ItemsViewModel.ListId)}={list.ListId}");
            }
            catch (Exception ex)
            {
                _logger.TrackError(ex);
            }
        }

        private async Task OnDeleteList(string listId)
        {
            try
            {
                var currentList = ListCollection.FirstOrDefault(l => l.ListId == listId);

                if (currentList == null) return;

                bool deleteList = await DialogService.DisplayAlert($"Delete list {currentList.Name}?", "This action cannot be undone.", "Yes", "No");

                if (deleteList)
                {
                    await DataStore.DeleteItemAsync(currentList.ListId);
                    await Shell.Current.GoToAsync($"..?{nameof(ListViewModel.ShouldRefresh)}={true}");
                }
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
                string newListName = await DialogService.DisplayPromptAsync("New list", String.Empty);

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

                await DataStore.AddItemAsync(list);

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
                var items = (await DataStore.GetItemsAsync(true)).Where(list => list.IsDeleted == IsDeleted);
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
                    await DataStore.UpdateItemAsync(list);
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