using ListApp.Helpers;
using ListApp.Models;
using ListApp.Resources;
using ListApp.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ListApp.ViewModels.Settings
{
    public class ThemeSelectionViewModel : BaseViewModel
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

        public ThemeSelectionViewModel()
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
                        case var value when value == typeof(ForestTheme).FullName:
                            SelectedTheme = Theme.Forest;
                            break;
                        case var value when value == typeof(RiverTheme).FullName:
                            SelectedTheme = Theme.River;
                            break;
                        case var value when value == typeof(BeeTheme).FullName:
                            SelectedTheme = Theme.Bee;
                            break;
                        case var value when value == typeof(QuartzoTheme).FullName:
                            SelectedTheme = Theme.Quartzo;
                            break;
                        case var value when value == typeof(NightTheme).FullName:
                            SelectedTheme = Theme.Night;
                            break;
                        case var value when value == typeof(InfernoTheme).FullName:
                            SelectedTheme = Theme.Inferno;
                            break;
                        default:
                            SelectedTheme = Theme.Forest;
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
                        Preferences.Set(PreferencesKeys.CurrentAppTheme, SelectedTheme.ToString());
                    }
                }
            }
        }
    }
}
