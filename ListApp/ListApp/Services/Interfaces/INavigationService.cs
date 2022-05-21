using System.Threading.Tasks;
using Xamarin.Forms;

namespace ListApp.Services.Interfaces
{
    public interface INavigationService
    {
        Task GoToAsync(string state, bool animate = false);
    }
}