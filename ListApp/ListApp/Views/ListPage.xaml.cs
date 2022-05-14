﻿using ListApp.ViewModels;
using Xamarin.Forms;

namespace ListApp.Views
{
    public partial class ListPage : ContentPage
    {
        ListViewModel _viewModel;

        public ListPage()
        {
            InitializeComponent();
            BindingContext = _viewModel = new ListViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }
    }
}