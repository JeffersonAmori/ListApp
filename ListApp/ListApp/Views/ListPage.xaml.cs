using ListApp.ViewModels;
using System.Threading.Tasks;
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

            int delay = 0;

            ListCollectionView.PreRevealAnimationAsync = async (viewCell) =>
            {
                viewCell.View.Opacity = 0;
                viewCell.View.Scale = 0;
            };

            ListCollectionView.RevealAnimationAsync = async (viewCell) =>
            {
                await Task.Delay(delay += 75);
                await Task.WhenAll(
                    viewCell.View.FadeTo(1),
                    viewCell.View.ScaleTo(1));
            };
        }
    }
}