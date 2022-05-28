using System.Linq;
using ListApp.Models;
using ListApp.Resources;
using ListApp.Resources.Internationalization;
using ListApp.Services.Interfaces;
using System.Collections.Generic;
using System.Globalization;
using Xamarin.CommunityToolkit.Helpers;

namespace ListApp.ViewModels.Settings
{
    internal class LanguageSelectionViewModel : BaseViewModel
    {
        private IList<Language> _languages;
        private Language _selectedLanguaged;
        private readonly IPreferencesService _preferenceService;

        public IList<Language> Languages
        {
            get => _languages;
            set => SetProperty(ref _languages, value);
        }

        public Language SelectedLanguage
        {
            get => _selectedLanguaged;
            set
            {
                _selectedLanguaged = value;
                UpdateLanguage();
                OnPropertyChanged();
            }
        }

        public LanguageSelectionViewModel(IPreferencesService preferenceService)
        {
            _preferenceService = preferenceService;

            LoadLanguages();
            string currentCI = _preferenceService.Get(PreferencesKeys.CurrentAppCulture, "en");
            SelectedLanguage = Languages.First(x => x.CI == currentCI);
        }

        private void LoadLanguages()
        {
            Languages = new List<Language>()
            {
                { new Language("English", "en") },
                { new Language("Português", "pt") }
            };
            SelectedLanguage = Languages.First(x => x.CI == LocalizationResourceManager.Current.CurrentCulture.TwoLetterISOLanguageName);
        }

        private void UpdateLanguage()
        {
            _preferenceService.Set(PreferencesKeys.CurrentAppCulture, SelectedLanguage.CI);
            LocalizationResourceManager.Current.CurrentCulture = CultureInfo.GetCultureInfo(SelectedLanguage.CI);
        }
    }
}
