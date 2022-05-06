using ListApp.Helpers;
using ListApp.Models;
using ListApp.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ListApp.ViewModels
{
    internal class SettingsViewModel : BaseViewModel
    {
        private Array _themes;
        private Theme _selectedTheme;

        public Array Themes
        {
            get => _themes;
            set
            {
                _themes = value;
                OnPropertyChanged();
            }
        }

        public Theme SelectedTheme
        {
            get => _selectedTheme;
            set
            {
                _selectedTheme = value;
                UpdateTheme(value);
                OnPropertyChanged();
            }
        }

        public SettingsViewModel()
        {
            Themes = Enum.GetValues(typeof(Theme));

            ICollection<ResourceDictionary> mergedDictionaries = Application.Current.Resources.MergedDictionaries;
            if (mergedDictionaries.Count > 0)
            {
                var currentTheme = mergedDictionaries.First().GetType();

                if (currentTheme.FullName != null)
                {
                    switch (currentTheme.FullName)
                    {
                        case var value when value == typeof(GreenTheme).FullName:
                            SelectedTheme = Theme.Green;
                            break;
                        case var value when value == typeof(BlueTheme).FullName:
                            SelectedTheme = Theme.Blue;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private void UpdateTheme(Theme value)
        {
            ICollection<ResourceDictionary> mergedDictionaries = Application.Current.Resources.MergedDictionaries;
            if (mergedDictionaries != null)
            {
                mergedDictionaries.Clear();

                // Parsing selected theme value
                if (Enum.TryParse(SelectedTheme.ToString(), out Theme currentThemeEnum))
                {
                    // Setting up theme
                    if (ThemeHelper.SetAppTheme(currentThemeEnum))
                    {
                        // Theme setting successful
                        Preferences.Set("CurrentAppTheme", SelectedTheme.ToString());
                    }
                }

            }
        }
    }
}
