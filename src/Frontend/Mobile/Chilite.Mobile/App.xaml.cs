using Chilite.Mobile.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Chilite.Mobile
{
    public partial class App 
    {
        public static string IPAddress = DeviceInfo.Platform == DevicePlatform.Android
            ? "10.0.2.2"
            : "localhost";

        public static string BaseUri = $"https://{IPAddress}:5001";
        
        public App()
        {
            InitializeComponent();

            DependencyService.Register<MockDataStore>();
            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}