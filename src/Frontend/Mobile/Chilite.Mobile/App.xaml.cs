using Chilite.Mobile.Services;
using Xamarin.Forms;

namespace Chilite.Mobile
{
    public partial class App 
    {
        public const string BaseUri = "https://localhost:5001/";

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