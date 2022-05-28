namespace ListApp.Services.Interfaces
{
    public interface IPreferencesService
    {
        void Remove(string key);
        void Set(string key, string value);
        string Get(string key, string defaultValue);
    }
}