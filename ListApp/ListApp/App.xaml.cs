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
using Xamarin.CommunityToolkit.Helpers;
using ListApp.Resources.Internationalization;
using System.Globalization;

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

                LocalizationResourceManager.Current.PropertyChanged += (sender, e) => LocalizedResources.Culture = LocalizationResourceManager.Current.CurrentCulture;
                LocalizationResourceManager.Current.Init(LocalizedResources.ResourceManager);
                LocalizationResourceManager.Current.CurrentCulture = new CultureInfo("en");

                MainPage = new AppShell();

                SetupServices();
                SetupJsonSerializer();
                SetupCurrentTheme();
                SetupCurrentUser();
                SetupCurrentCulture();
            }
            catch (Exception ex)
            {
                var logger = ServiceProvider.GetRequiredService<ILogger>() ?? new AppCenterLogger();
                logger.TrackError(ex);
            }
        }

        private void SetupCurrentCulture()
        {
            LocalizationResourceManager.Current.CurrentCulture = CultureInfo.GetCultureInfo(Preferences.Get(PreferencesKeys.CurrentAppCulture, "en"));
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
        private void SetupServices()
        {
            var services = new ServiceCollection();

            services
                .AddTransient<ILogger, AppCenterLogger>()
                .AddTransient<IDialogService, DialogService>()
                .AddTransient<IDataStore<List>, EfListDataStore>()
                .AddTransient<IDataStore<ListItem>, EfListItemDataStore>()
                .AddTransient<INavigationService, NavigationService>()
                .AddTransient<IShareService, ShareService>()
                .AddSingleton<IHttpClientService>(new HttpClientService())
                .AddTransient<IWebAuthenticatorService, WebAuthenticatorService>()
                .AddTransient<IPreferencesService, PreferencesService>()
                .AddTransient<ListContext>()
                .AddTransient<ItemsViewModel>()
                .AddTransient<ListViewModel>()
                .AddTransient<AccountManagementViewModel>()
                .AddTransient<ThemeSelectionViewModel>()
                .AddTransient<LanguageSelectionViewModel>();
            

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
