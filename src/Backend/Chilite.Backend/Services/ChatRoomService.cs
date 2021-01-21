using Chilite.Protos;
using Grpc.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using Chilite.Database;
using ChatMessage = Chilite.Protos.ChatMessage;

namespace Chilite.Backend
{
    public class ChatRoomService : ChatRoom.ChatRoomBase
    {
        #region Private Fields

        private readonly ChatRoomManager _chatRoomManager;
        private readonly ChatDbContext _chatDbContext;

        private readonly List<IServerStreamWriter<ChatMessage>> _listeners = new();

        #endregion

        #region Constructor

        public ChatRoomService(ChatRoomManager chatRoomManager, ChatDbContext chatDbContext)
        {
            _chatRoomManager = chatRoomManager;
            _chatDbContext = chatDbContext;

            _chatRoomManager.MessageSended += ChatRoomService_MessageSended;
        }

        #endregion

        #region Public Methods

        public override async Task JoinChat(ChatRequest request, IServerStreamWriter<ChatMessage> responseStream,
            ServerCallContext context)
        {
            foreach (var chatMessage in _chatDbContext.Messages)
            {
                await responseStream.WriteAsync(new ChatMessage {Message = chatMessage.Message});
            }

            _listeners.Add(responseStream);

            while (!context.CancellationToken.IsCancellationRequested)
            {
                await Task.Delay(100);
            }

            _listeners.Remove(responseStream);
        }

        public override async Task<ChatRequest> Send(ChatMessage request, ServerCallContext context)
        {
            var chatMessage = new Chilite.Database.ChatMessage
            {
                Message = request.Message
            };

            await _chatRoomManager.AddMessageAsync(chatMessage);

            await _chatDbContext.Messages.AddAsync(chatMessage);
            await _chatDbContext.SaveChangesAsync();

            return new ChatRequest();
        }

        #endregion

        #region Private Methods

        private void ChatRoomService_MessageSended(string message)
        {
            foreach (var streamWriter in _listeners)
            {
                streamWriter.WriteAsync(new ChatMessage
                {
                    Message = message
                });
            }
        }

        #endregion
    }
}