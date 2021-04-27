using Chilite.Database;
using Chilite.Protos;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using ChatMessage = Chilite.Protos.ChatMessage;

namespace Chilite.Backend
{
    [Authorize]
    public class ChatRoomService : ChatRoom.ChatRoomBase
    {
        #region Private Fields

        private readonly ChatRoomManager _chatRoomManager;
        private readonly ChatDbContext _chatDbContext;
        private readonly UserManager<ChatUser> _userManager;

        private readonly List<IServerStreamWriter<ChatMessage>> _listeners = new();

        #endregion

        #region Constructor

        public ChatRoomService(ChatRoomManager chatRoomManager, ChatDbContext chatDbContext ,UserManager<ChatUser> userManager)
        {
            _chatRoomManager = chatRoomManager;
            _chatDbContext = chatDbContext;
            _userManager = userManager;

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
            
            await Task.Delay(int.MaxValue ,context.CancellationToken);
            
            _listeners.Remove(responseStream);
        }

        public override async Task<ChatRequest> Send(ChatMessage request, ServerCallContext context)
        {
            var user = await _userManager.GetUserAsync(context.GetHttpContext().User);

            var chatMessage = new Chilite.Database.ChatMessage
            {
                Message = request.Message,
                User = user
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