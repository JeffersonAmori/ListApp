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
using ListApp.ViewModels.Settings;
using Microsoft.Extensions.DependencyInjection;
using ListApp.ViewModels;

namespace ListApp
{
    public partial class App : Application
    {
        protected static IServiceProvider ServiceProvider { get; set; }

        public static BaseViewModel GetViewModel<TViewModel>() where TViewModel : BaseViewModel
            => ServiceProvider.GetService<TViewModel>();

        public App()
        {
            try
            {
                InitializeComponent();

                Sharpnado.CollectionView.Initializer.Initialize(true, false);

                MainPage = new AppShell();

                SetupServices();
                SetupJsonSerializer();
                SetupCurrentTheme();
                SetupCurrentUser();
            }
            catch (Exception ex)
            {
                var logger = ServiceProvider.GetRequiredService<ILogger>() ?? new AppCenterLogger();
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
        /// Set up the services used by the app.
        /// </summary>
        void SetupServices()
        {
            var services = new ServiceCollection();

            services.AddTransient<ILogger, AppCenterLogger>();
            services.AddTransient<IDialogService, DialogService>();
            services.AddTransient<IDataStore<List>, EfListDataStore>();
            services.AddTransient<IDataStore<ListItem>, EfListItemDataStore>();
            services.AddTransient<INavigationService, NavigationService>();
            services.AddTransient<ListContext>();
            services.AddTransient<ItemsViewModel>();
            services.AddTransient<ListViewModel>();
            services.AddTransient<AccountManagementViewModel>();
            services.AddTransient<ThemeSelectionViewModel>();

            ServiceProvider = services.BuildServiceProvider();
        }

        /// <summary>
        /// Set up the options for the JSonSerializer class.
        /// </summary>
        private static void SetupJsonSerializer()
        {
            ((JsonSerializerOptions)typeof(JsonSerializerOptions)
                .GetField("s_defaultOptions",
                    System.Reflection.BindingFlags.Static |
                    System.Reflection.BindingFlags.NonPublic).GetValue(null))
                .PropertyNameCaseInsensitive = true;
        }

        /// <summary>
        /// Set up current theme from app settings.
        /// </summary>
        public void SetupCurrentTheme()
        {
            if (Preferences.Get(PreferencesKeys.CurrentAppTheme, null) is string currentTheme)
                if (Enum.TryParse(currentTheme, out Theme currentThemeEnum))
                    ThemeHelper.SetAppTheme(currentThemeEnum);
        }

        /// <summary>
        /// Load the current logged in user from app settings.
        /// </summary>
        private void SetupCurrentUser()
        {
            if (Preferences.Get(PreferencesKeys.ApplicationUserInfo, null) is string user)
                ApplicationUser.Current.Set(
                    JsonSerializer.Deserialize<ApplicationUser>(user));
        }
    }
}
