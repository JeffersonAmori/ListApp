﻿using ListApp.Helpers;
using ListApp.Models;
using ListApp.Resources;
using ListApp.Services.Interfaces;
using ListApp.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace ListApp.ViewModels.Settings
{
    public class ThemeSelectionViewModel : BaseViewModel
    {
        private readonly IPreferencesService _preferenceService;
        private readonly ILogger _logger;
        private Array _themes;
        private Theme _selectedTheme;

        public Array Themes
        {
            get => _themes;
            set=> SetProperty(ref _themes, value);
        }

        public Theme SelectedTheme
        {
            get => _selectedTheme;
            set
            {
                _selectedTheme = value;
                _logger.TrackEvent(Events.ThemeChanged);
                UpdateTheme();
                OnPropertyChanged();
            }
        }

        public ThemeSelectionViewModel(IPreferencesService preferenceService, ILogger logger)
        {
            _preferenceService = preferenceService;
            _logger = logger;

            try
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
                            case var value when value == typeof(LondonTheme).FullName:
                                SelectedTheme = Theme.London;
                                break;
                            default:
                                SelectedTheme = Theme.Forest;
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.TrackError(ex);
            }
        }

        private void UpdateTheme()
        {
            try
            {
                ICollection<ResourceDictionary> mergedDictionaries = Application.Current.Resources.MergedDictionaries;

                if (mergedDictionaries == null)
                    return;

                mergedDictionaries.Clear();

                if (!Enum.TryParse(SelectedTheme.ToString(), out Theme currentThemeEnum))
                    return;

                if (ThemeHelper.SetAppTheme(currentThemeEnum))
                    _preferenceService.Set(PreferencesKeys.CurrentAppTheme, SelectedTheme.ToString());
            }
            catch (Exception ex)
            {
                _logger.TrackError(ex);
            }
        }
    }
}
