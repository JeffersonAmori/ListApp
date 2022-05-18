using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using ListApp.Helpers;
using ListApp.Models;
using ListApp.Resources;
using ListApp.Services;
using System;
using System.Text.Json;
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
            DependencyService.RegisterSingleton(new DialogService());

            ((JsonSerializerOptions)typeof(JsonSerializerOptions)
                .GetField("s_defaultOptions",
                    System.Reflection.BindingFlags.Static |
                    System.Reflection.BindingFlags.NonPublic).GetValue(null))
                .PropertyNameCaseInsensitive = true;

            MainPage = new AppShell();

            SetupCurrentTheme();
            SetupCurrentUser();
        }

        protected override void OnStart()
        {

            AppCenter.Start($"android={Secrets.AppCenterAndroidAppSecret};",
                     typeof(Analytics), typeof(Crashes));

#if DEBUG
            AppCenter.SetEnabledAsync(false);
#endif

            Analytics.TrackEvent("App start");
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
            try
            {
                if (Preferences.Get(PreferencesKeys.CurrentAppTheme, null) is string currentTheme)
                    if (Enum.TryParse(currentTheme, out Theme currentThemeEnum))
                        ThemeHelper.SetAppTheme(currentThemeEnum);
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// Load the current logged in user from app settings
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void SetupCurrentUser()
        {
            if (Preferences.Get(PreferencesKeys.ApplicationUserInfo, null) is string user)
                ApplicationUser.Current.Set(
                    JsonSerializer.Deserialize<ApplicationUser>(user));
        }
    }
}
