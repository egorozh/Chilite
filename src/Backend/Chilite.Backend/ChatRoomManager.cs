using Chilite.Database;
using System;
using System.Threading.Tasks;

namespace Chilite.Backend
{
    public class ChatRoomManager
    {
        public event Action<string> MessageSended;

        public async Task AddMessageAsync(ChatMessage chatMessage)
        {
            MessageSended?.Invoke(chatMessage.Message);
        }
    }
}