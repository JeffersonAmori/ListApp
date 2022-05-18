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
using ListApp.Services.Interfaces;

namespace ListApp
{
    public partial class App : Application
    {

        public App()
        {
            try
            {
                InitializeComponent();

                DependencyService.RegisterSingleton(new AppCenterLogger());
                DependencyService.RegisterSingleton(new DialogService());
                DependencyService.Register<EfListDataStore>();
                DependencyService.Register<EfListItemDataStore>();

                Sharpnado.CollectionView.Initializer.Initialize(true, false);

                ((JsonSerializerOptions)typeof(JsonSerializerOptions)
                    .GetField("s_defaultOptions",
                        System.Reflection.BindingFlags.Static |
                        System.Reflection.BindingFlags.NonPublic).GetValue(null))
                    .PropertyNameCaseInsensitive = true;

                MainPage = new AppShell();

                SetupCurrentTheme();
                SetupCurrentUser();
            }
            catch (Exception ex)
            {
                var logger = DependencyService.Get<ILogger>();
                logger.TrackError(ex);
            }
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
                if (Preferences.Get(PreferencesKeys.CurrentAppTheme, null) is string currentTheme)
                    if (Enum.TryParse(currentTheme, out Theme currentThemeEnum))
                        ThemeHelper.SetAppTheme(currentThemeEnum);
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
