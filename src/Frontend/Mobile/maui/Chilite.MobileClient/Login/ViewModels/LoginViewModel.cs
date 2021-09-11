using System.Net.Http;
using Chilite.FrontendCore;
using Chilite.MobileClient.Chat.Views;
using Chilite.Protos;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Microsoft.Maui.Controls;

namespace Chilite.MobileClient.Login.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private string _login;
        private string _password;

        #region Public Properties

        public string Login
        {
            get => _login;
            set => SetProperty(ref _login, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        #endregion

        public Command LoginCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new Command(OnLoginClicked);
        }

        private async void OnLoginClicked(object obj)
        {
            if (!string.IsNullOrEmpty(Login) && !string.IsNullOrEmpty(Password))
            {
                try
                {
                    var acclient = GetAccountClient(App.BaseUri);

                    var tokenResponse = await acclient.LoginAsync(new LoginRequest()
                    {
                        Login = Login,
                        Password = Password
                    });

                    if (tokenResponse.ResultCase == LoginResponse.ResultOneofCase.Login)
                    {
                        var token = tokenResponse.Login.Token;

                        await Shell.Current.GoToAsync($"//{nameof(ChatPage)}?Token={token}");
                    }
                }
                catch (Exception e)
                {
                }
            }
        }
        
        //private static Account.AccountClient GetAccountClient(string baseUri)
        //    => new(GetAuthChannel(baseUri));

        public static GrpcChannel GetAuthChannel(string baseUri, string? token = null)
        {
            var httpClientHandler = new SocketsHttpHandler();

            //httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
            //{
            //    return true;
            //};

            return new HttpClient(httpClientHandler)
                .ToAuthChannel(baseUri, token);
        }
        private static Account.AccountClient GetAccountClient(string baseUri)
        {
            var channel = GrpcChannel.ForAddress(baseUri, new GrpcChannelOptions
            {
                HttpHandler = new SocketsHttpHandler()
            });

            return new Account.AccountClient(channel);
        }
    }
}