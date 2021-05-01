using Chilite.ViewModels.Events;
using Prism.Events;

namespace Chilite.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public const string BaseUri = "https://localhost:5001/";
            
        public object CurrentPage { get; set; }

        public MainViewModel(IEventAggregator eventAggregator)
        {
            CurrentPage = new LoginViewModel(eventAggregator);

            eventAggregator.GetEvent<LoginEvent>().Subscribe(OnLogin);
        }

        private void OnLogin(string token)
        {   
            CurrentPage = new ChatViewModel(token);
        }
    }
}