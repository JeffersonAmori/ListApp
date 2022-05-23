using Xamarin.UITest;

namespace ListApp.UITests
{
    public class AppInitializer
    {
        public static IApp StartApp(Platform platform)
        {
            if (platform == Platform.Android)
            {
                return ConfigureApp
                            .Android
                            .EnableLocalScreenshots()
                            .InstalledApp("com.deviance.listfreak")
                            .StartApp();
            }

            return ConfigureApp.iOS.StartApp();
        }
    }
}