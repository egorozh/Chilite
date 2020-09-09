using System;

namespace Chilite.Database
{
    public class ChatMessage
    {
        public string Id { get; set; }

        public string Message { get; set; }

        public ChatMessage()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}