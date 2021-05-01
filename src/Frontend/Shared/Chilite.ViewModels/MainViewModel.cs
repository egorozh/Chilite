using Prism.Events;

namespace Chilite.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public object CurrentPage { get; set; }

        public MainViewModel(IEventAggregator eventAggregator)
        {
            CurrentPage = new LoginViewModel(eventAggregator);

            eventAggregator.GetEvent<LoginEvent>().Subscribe(OnLogin);
        }

        private void OnLogin()
        {   
            CurrentPage = new ChatViewModel();
        }
    }
}