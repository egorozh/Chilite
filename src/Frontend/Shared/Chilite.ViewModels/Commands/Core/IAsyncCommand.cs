using System.Threading.Tasks;
using System.Windows.Input;

namespace Chilite.ViewModels
{
    public interface IAsyncCommand : ICommand
    {
        Task ExecuteAsync();
        bool CanExecute();
    }
}