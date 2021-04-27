using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using PropertyChanged;

namespace Chilite.Desktop.Views
{
    [DoNotNotify]
    public class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}