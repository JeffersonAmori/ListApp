﻿using ListApp.Views;
using System;
using Xamarin.Forms;

namespace ListApp
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(ItemsPage), typeof(ItemsPage));
        }
    }
}