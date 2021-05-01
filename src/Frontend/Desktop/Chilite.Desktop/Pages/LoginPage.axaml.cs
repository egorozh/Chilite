using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Chilite.Desktop.Pages
{
    public class LoginPage : UserControl
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
