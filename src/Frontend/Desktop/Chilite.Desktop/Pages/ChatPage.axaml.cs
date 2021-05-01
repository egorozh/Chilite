using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Chilite.Desktop.Pages
{
    public class ChatPage : UserControl
    {
        public ChatPage()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
