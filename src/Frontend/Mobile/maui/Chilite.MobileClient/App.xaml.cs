using Microsoft.Maui.Essentials;
using Application = Microsoft.Maui.Controls.Application;

namespace Chilite.MobileClient
{
    public partial class App : Application
    {
        public static string IPAddress = DeviceInfo.Platform == DevicePlatform.Android
            ? "10.0.2.2"
            : "localhost";

        public static string BaseUri = $"https://{IPAddress}:5001";

        public App()
        {
            InitializeComponent();
                    
            MainPage = new AppShell();
        }
    }
}
