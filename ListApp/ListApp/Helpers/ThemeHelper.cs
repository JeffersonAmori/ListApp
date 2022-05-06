using ListApp.Models;
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
                    case Theme.Green:
                        mergedDictionaries.Add(new GreenTheme());
                        break;

                    case Theme.Blue:
                        mergedDictionaries.Add(new BlueTheme());
                        break;

                    default:
                        mergedDictionaries.Add(new GreenTheme());
                        break;
                }

                return true;
            }

            return false;
        }
    }
}