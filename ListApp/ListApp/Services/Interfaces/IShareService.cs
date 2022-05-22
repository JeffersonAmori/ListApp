using System.Threading.Tasks;

namespace ListApp.Services.Interfaces
{
    public interface IShareService
    {
        Task RequestAsync(string text, string title);
    }
}