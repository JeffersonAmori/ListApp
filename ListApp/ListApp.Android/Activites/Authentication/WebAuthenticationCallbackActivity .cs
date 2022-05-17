using Android.App;
using Android.Content;
using Android.Content.PM;

namespace ListApp.Droid.Activites.Authentication
{
    [Activity(NoHistory = true, LaunchMode = LaunchMode.SingleTop)]
    [IntentFilter(new[] { Intent.ActionView }, Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable }, DataScheme = CALLBACK_SCHEME)]
    public class WebAuthenticationCallbackActivity : Xamarin.Essentials.WebAuthenticatorCallbackActivity
    {
        const string CALLBACK_SCHEME = "listfreak";
    }
}