using System.Threading.Tasks;
using Chilite.MobileClient.Login.ViewModels;
using Microsoft.Maui.Controls;

namespace Chilite.MobileClient.Login.Views
{
    public partial class LoginPage : ContentPage, IAllertService
    {
        public LoginPage()
        {
            InitializeComponent();

            BindingContext = new LoginViewModel(this);
        }

        public async Task ShowAllert(string message)
        {
            await DisplayAlert("Ошибка", message, "Ок");
        }
    }

    public interface IAllertService
    {
        Task ShowAllert(string message);
    }
}