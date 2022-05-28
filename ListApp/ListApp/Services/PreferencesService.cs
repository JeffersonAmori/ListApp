using ListApp.Services.Interfaces;
using Xamarin.Essentials;

namespace ListApp.Services
{
    internal class PreferencesService : IPreferencesService
    {
        public void Set(string key, string value) =>
            Preferences.Set(key, value);

        public void Remove(string key) =>
            Preferences.Remove(key);

        public string Get(string key, string defaultValue) =>
            Preferences.Get(key, defaultValue);
    }
}
