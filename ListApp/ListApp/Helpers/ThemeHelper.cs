﻿using ListApp.Models;
using ListApp.Themes;
using System.Collections.Generic;
using Xamarin.Forms;

namespace ListApp.Helpers
{
    internal static class ThemeHelper
    {
        internal static bool SetAppTheme(Theme selectedTheme)
        {
            ICollection<ResourceDictionary> mergedDictionaries = Application.Current.Resources.MergedDictionaries;
            if (mergedDictionaries != null)
            {
                mergedDictionaries.Clear();

                switch (selectedTheme)
                {
                    case Theme.Forest:
                        mergedDictionaries.Add(new ForestTheme());
                        break;
                    case Theme.River:
                        mergedDictionaries.Add(new RiverTheme());
                        break;
                    case Theme.Bee:
                        mergedDictionaries.Add(new BeeTheme());
                        break;
                    case Theme.Quartzo:
                        mergedDictionaries.Add(new QuartzoTheme());
                        break;
                    case Theme.Night:
                        mergedDictionaries.Add(new NightTheme());
                        break;
                    case Theme.Inferno:
                        mergedDictionaries.Add(new InfernoTheme());
                        break;
                    case Theme.London:
                        mergedDictionaries.Add(new LondonTheme());
                        break;
                    default:
                        mergedDictionaries.Add(new ForestTheme());
                        break;
                }

                return true;
            }

            return false;
        }
    }
}