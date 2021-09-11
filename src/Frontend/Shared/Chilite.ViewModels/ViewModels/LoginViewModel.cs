using Chilite.Protos;
using Chilite.ViewModels.Events;
using Grpc.Net.Client;
using Prism.Events;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Chilite.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        #region Private Fields

        private readonly IEventAggregator _eventAggregator;

        #endregion

        #region Public Properties

        public string Login { get; set; }

        public string Password { get; set; }

        #endregion

        #region Commands

        public ICommand LoginCommand { get; }

        #endregion

        #region Constructor

        public LoginViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            LoginCommand = new AsyncCommand(OnLogin);
        }

        #endregion

        #region Private Methods

        private async Task OnLogin()
        {
            if (!string.IsNullOrEmpty(Login) && !string.IsNullOrEmpty(Password))
            {
                var acclient = GetAccountClient(MainViewModel.BaseUri);

                var tokenResponse = await acclient.LoginAsync(new LoginRequest()
                {
                    Login = Login,
                    Password = Password
                });

                if (tokenResponse.ResultCase == LoginResponse.ResultOneofCase.Login)
                {
                    var token = tokenResponse.Login.Token;
                    
                    _eventAggregator.GetEvent<LoginEvent>().Publish(token);
                }
            }
        }

        private static Account.AccountClient GetAccountClient(string baseUri)
        {
            var channel = GrpcChannel.ForAddress(baseUri, new GrpcChannelOptions
            {
                HttpHandler = new SocketsHttpHandler()
            });

            return new Account.AccountClient(channel);
        }

        #endregion
    }
}