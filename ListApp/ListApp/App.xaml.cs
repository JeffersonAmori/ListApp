using ListApp.Services;
using Xamarin.Forms;

namespace ListApp
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            //DependencyService.Register<MockListDataStore>();
            //DependencyService.Register<MockListItemDataStore>();
            DependencyService.Register<EfListDataStore>();
            DependencyService.Register<EfListItemDataStore>();

            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
