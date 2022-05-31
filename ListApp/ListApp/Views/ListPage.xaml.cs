using ListApp.ViewModels;
using Xamarin.Forms;

namespace ListApp.Views
{
    public partial class ListPage : ContentPage
    {
        ListViewModel _viewModel;

        public ListPage()
        {
            InitializeComponent();
            BindingContext = _viewModel = (ListViewModel)App.GetViewModel<ListViewModel>();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();

            //ListCollectionView.DragAndDropEnabledAnimationAsync = async (viewCell, token) =>
            //{
            //    while (!token.IsCancellationRequested)
            //    {
            //        await viewCell.View.RotateTo(8);
            //        await viewCell.View.RotateTo(-8);
            //    }

            //    await viewCell.View.RotateTo(0);
            //};

            ListCollectionView.PreRevealAnimationAsync = async (viewCell) =>
            {
                viewCell.View.Opacity = 0;
                viewCell.View.RotationX = 90;
            };

            ListCollectionView.RevealAnimationAsync = async (viewCell) =>
            {
                await viewCell.View.FadeTo(1);
                await viewCell.View.RotateXTo(0);
            };
        }
    }
}