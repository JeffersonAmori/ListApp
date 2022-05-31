using ListApp.Models;
using ListApp.ViewModels;
using Xamarin.Forms;

namespace ListApp.Views
{
    public partial class ItemsPage : ContentPage
    {
        ItemsViewModel _viewModel;

        public ItemsPage()
        {
            InitializeComponent();
            BindingContext = _viewModel = (ItemsViewModel)App.GetViewModel<ItemsViewModel>();
        }

        protected override void OnAppearing()
        {
            ItemsListView.PreRevealAnimationAsync = async (viewCell) =>
            {
                viewCell.View.Opacity = 0;
                viewCell.View.RotationX = 90;
            };

            ItemsListView.RevealAnimationAsync = async (viewCell) =>
            {
                await viewCell.View.FadeTo(1);
                await viewCell.View.RotateXTo(0);
            };
        }

        private void ListItemDescriptionEntry_Completed(object sender, System.EventArgs e)
        {
            Entry entry = sender as Entry;

            if (entry == null) return;

            ListItem listItem = entry.Parent.BindingContext as ListItem;

            if (listItem == null) return;

            if (_viewModel.CompletedListItemEntryCommand.CanExecute(listItem))
            {
                _viewModel.CompletedListItemEntryCommand.Execute(listItem);
                entry.Text = string.Empty;
                entry.Focus();
            }
        }
    }
}