using ListApp.ViewModels.Settings;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ListApp.Views.Settings
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AccountManagementPage : ContentPage
    {
        public AccountManagementPage()
        {
            InitializeComponent();
            BindingContext = App.GetViewModel<AccountManagementViewModel>();
        }
    }
}