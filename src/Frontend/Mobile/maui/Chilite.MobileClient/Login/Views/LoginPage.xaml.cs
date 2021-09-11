using Chilite.MobileClient.Login.ViewModels;
using Microsoft.Maui.Controls;

namespace Chilite.MobileClient.Login.Views
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();

            BindingContext = new LoginViewModel();
        }
    }
}