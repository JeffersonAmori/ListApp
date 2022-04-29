using ListApp.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace ListApp.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}