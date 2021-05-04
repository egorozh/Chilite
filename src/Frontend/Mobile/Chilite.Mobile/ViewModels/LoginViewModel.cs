using System;
using System.Net.Http;
using Chilite.Mobile.Views;
using Chilite.Protos;
using Grpc.Net.Client;
using Xamarin.Forms;

namespace Chilite.Mobile.ViewModels
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

        private static Account.AccountClient GetAccountClient(string baseUri)
        {
            var channel = GrpcChannel.ForAddress(baseUri, new GrpcChannelOptions
            {
                HttpHandler = new HttpClientHandler()
            });

            return new Account.AccountClient(channel);
        }
    }
}