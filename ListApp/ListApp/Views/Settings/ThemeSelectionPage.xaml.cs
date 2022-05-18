using ListApp.ViewModels.Settings;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ListApp.Views.Settings
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ThemeSelectionPage : ContentPage
	{
		public ThemeSelectionPage()
		{
			InitializeComponent();
			BindingContext = App.GetViewModel<ThemeSelectionViewModel>();
		}
	}
}