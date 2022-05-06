using ListApp.Models;
using Xamarin.Forms;

namespace ListApp.Themes
{
    //[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BaseTheme : ResourceDictionary
    {
        internal protected Theme Theme { get; protected set; } = Theme.Green;
        public BaseTheme() 
        {
            InitializeComponent();
        }
    }
}