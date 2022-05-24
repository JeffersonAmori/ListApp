using ListApp.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;

namespace ListApp.Services
{
    internal class PreferencesService : IPreferencesService
    {
        public void Set(string key, string value) =>
            Preferences.Set(key, value);

        public void Remove(string key) =>
            Preferences.Remove(key);
    }
}
