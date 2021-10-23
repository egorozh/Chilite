namespace Chilite.DomainModels;

public class ChatMessage
{
    public string Id { get; set; }

    public string Message { get; set; }

    public ChatUser User { get; set; }  

    public ChatMessage()
    {
        Id = Guid.NewGuid().ToString();
    }
}