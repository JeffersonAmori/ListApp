using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ListApp.Views
{
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            this.ListListView.SelectedItem = null;
        }
    }
}