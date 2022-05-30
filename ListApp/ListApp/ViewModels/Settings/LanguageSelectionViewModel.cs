using ListApp.Models;
using ListApp.Resources;
using ListApp.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Xamarin.CommunityToolkit.Helpers;

namespace ListApp.ViewModels.Settings
{
    internal class LanguageSelectionViewModel : BaseViewModel
    {
        private IList<Language> _languages;
        private Language _selectedLanguaged;
        private readonly IPreferencesService _preferenceService;
        private readonly ILogger _logger;

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
                _logger.TrackEvent(Events.LanguageChanged);
                UpdateLanguage();
                OnPropertyChanged();
            }
        }

        public LanguageSelectionViewModel(IPreferencesService preferenceService, ILogger logger)
        {
            _preferenceService = preferenceService;
            _logger = logger;

            try
            {
                LoadLanguages();
                string currentCI = _preferenceService.Get(PreferencesKeys.CurrentAppCulture, "en");
                SelectedLanguage = Languages.First(x => x.CI == currentCI);
            }
            catch (Exception ex)
            {
                _logger.TrackError(ex);
            }
        }

        private void LoadLanguages()
        {
            try
            {
                Languages = Language.KnownLanguages.ToList();
                SelectedLanguage = Languages.First(x => x.CI == LocalizationResourceManager.Current.CurrentCulture.TwoLetterISOLanguageName);
            }
            catch (Exception ex)
            {
                _logger.TrackError(ex);
            }
        }

        private void UpdateLanguage()
        {
            try
            {
                _preferenceService.Set(PreferencesKeys.CurrentAppCulture, SelectedLanguage.CI);
                LocalizationResourceManager.Current.CurrentCulture = CultureInfo.GetCultureInfo(SelectedLanguage.CI);
            }
            catch (Exception ex)
            {
                _logger.TrackError(ex);
            }
        }
    }
}
