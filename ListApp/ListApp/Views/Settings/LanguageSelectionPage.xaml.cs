using ListApp.ViewModels.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ListApp.Views.Settings
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LanguageSelectionPage : ContentPage
    {
        public LanguageSelectionPage()
        {
            InitializeComponent();
            BindingContext = App.GetViewModel<LanguageSelectionViewModel>();
        }
    }
}