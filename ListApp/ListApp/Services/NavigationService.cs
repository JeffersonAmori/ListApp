using ListApp.Services.Interfaces;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ListApp.Services
{
    public class NavigationService : INavigationService
    {
        public async Task GoToAsync(string state, bool animate = false)
        {
            await Shell.Current.GoToAsync(state, animate);
        }
    }
}
