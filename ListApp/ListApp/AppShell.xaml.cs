using ListApp.Views;
using ListApp.Views.Settings;
using Xamarin.Forms;

namespace ListApp
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(ItemsPage), typeof(ItemsPage));
            Routing.RegisterRoute(nameof(ThemeSelectionPage), typeof(ThemeSelectionPage));
        }
    }
}
