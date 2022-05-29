using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.AppCompat.App;

namespace ListApp.Droid.Activites.SplashScreen
{
    [Activity(Theme = "@style/SplashScreenTheme", MainLauncher = true, NoHistory = true)]
    public class SplashScreenActivity : AppCompatActivity
    {
        public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
        {
            base.OnCreate(savedInstanceState, persistentState);
        }

        protected override void OnResume()
        {
            base.OnResume();
            StartActivity(new Intent(Application.Context, typeof(MainActivity)));
        }

        public override void OnBackPressed() { }
    }
}