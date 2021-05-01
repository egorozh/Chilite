using Chilite.Protos;
using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Chilite.ViewModels
{
    public class ChatViewModel : BaseViewModel
    {
        private readonly ChatRoom.ChatRoomClient _chatClient;

        public ObservableCollection<string> Messages { get; } = new();

        public string MessageText { get; set; }
            
        public ICommand SendCommand { get; }

        public ChatViewModel(string token)
        {
            _chatClient = GetChatRoomClient(MainViewModel.BaseUri, token);

            SendCommand = new AsyncCommand(OnSend);

            Init();
        }

        private async Task OnSend()
        {
            if (string.IsNullOrEmpty(MessageText))
                return;

            await _chatClient.SendAsync(new ChatMessage
            {
                Message = MessageText
            });

            MessageText = "";
        }

        private async void Init()
        {
            var serverStream = _chatClient.JoinChat(new ChatRequest());
            var stream = serverStream.ResponseStream;

            try
            {
                await foreach (var message in stream.ReadAllAsync())
                {
                    Messages.Add(message.Message);
                }
            }
            catch (Exception e)
            {
            }
            finally
            {
                serverStream.Dispose();
            }
        }
        
        private static ChatRoom.ChatRoomClient GetChatRoomClient(string baseUri, string token)
            => new(GetAuthChannel(baseUri, token));

        public static GrpcChannel GetAuthChannel(string baseUri, string token) =>
            GrpcChannel.ForAddress(baseUri,
                new GrpcChannelOptions
                {
                    HttpClient = new HttpClient(),
                    Credentials = ChannelCredentials.Create(new SslCredentials(),
                        GetJwtCredentials(token))
                });

        private static CallCredentials GetJwtCredentials(string token) =>
            CallCredentials.FromInterceptor((_, metadata) =>
            {
                AddJwt(metadata, token);

                return Task.CompletedTask;
            });

        private static void AddJwt(Metadata metadata, string token)
        {
            if (!string.IsNullOrEmpty(token))
                metadata.Add("Authorization", $"Bearer {token}");
        }
    }
}