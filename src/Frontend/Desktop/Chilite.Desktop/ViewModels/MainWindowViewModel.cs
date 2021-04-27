using Chilite.Protos;
using Grpc.Core;
using Grpc.Net.Client;
using Prism.Commands;
using System.Net.Http;
using System.Threading.Tasks;

namespace Chilite.Desktop.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public string Messages { get; set; }

        public DelegateCommand ConnectCommand { get; }

        public MainWindowViewModel()
        {
            const string baseUri = "https://localhost:5001/";
            ConnectCommand = new DelegateCommand(async () => { await Connect(baseUri); });
        }

        private async Task Connect(string baseUri)
        {
            var acclient = GetAccountClient(baseUri);

            var tokenResponse = await acclient.LoginAsync(new LoginRequest()
            {
                Login = "123",
                Password = "123"
            });

            if (tokenResponse.ResultCase == LoginResponse.ResultOneofCase.Login)
            {
                var token = tokenResponse.Login.Token;

                Messages += "\r\n" + token;

                Metadata headers = new();
                if (!string.IsNullOrEmpty(token))
                    headers.Add("Authorization", $"Bearer {token}");

                var res = await acclient.GetUserProfileAsync(new UserInfoRequest(), headers);

                Messages += "\r\n" + res.Profile.Username;
            }
        }

        private static Account.AccountClient GetAccountClient(string baseUri)
        {
            GrpcChannel channel = GrpcChannel.ForAddress(baseUri, new GrpcChannelOptions
            {
                HttpHandler = new HttpClientHandler()
            });

            return new Account.AccountClient(channel);
        }
    }
}