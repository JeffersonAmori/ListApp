using ListApp.Helpers;
using ListApp.Models;
using ListApp.Resources;
using ListApp.Services;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ListApp
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            Sharpnado.CollectionView.Initializer.Initialize(true, false);

            DependencyService.Register<EfListDataStore>();
            DependencyService.Register<EfListItemDataStore>();
            DependencyService.Register<DialogService>();

            MainPage = new AppShell();

            SetupCurrentTheme();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

        /// <summary>
        /// Set up current theme from app settings
        /// </summary>
        public void SetupCurrentTheme()
        {
            var currentTheme = Preferences.Get(PreferencesKeys.CurrentAppTheme, null);

            if (currentTheme == null)
                return;

            if (Enum.TryParse(currentTheme, out Theme currentThemeEnum))
            {
                ThemeHelper.SetAppTheme(currentThemeEnum);
            }
        }
    }
}
