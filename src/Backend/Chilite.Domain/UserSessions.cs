using Chilite.Database;
using Chilite.DomainModels;
using Microsoft.Extensions.DependencyInjection;

namespace Chilite.Domain;

public class UserSessions
{
    private readonly ChatApp _chatApp;

    public event Action<ChatMessage> NewMessageSended;

    public UserSessions(ChatApp chatApp, ChatUser user)
    {
        _chatApp = chatApp;
    }

    public void Init()
    {
        using var scope = _chatApp.ServiceProvider.CreateScope();

        var chatDbContext = scope.ServiceProvider.GetRequiredService<ChatDbContext>();

        foreach (var chatMessage in chatDbContext.Messages)
        {
            NewMessageSended?.Invoke(chatMessage);
        }
    }

    public void Dispose()
    {
        _chatApp.Sessions.Remove(this);
    }

    public void MessageInvoked(ChatMessage chatMessage)
    {
        NewMessageSended?.Invoke(chatMessage);
    }
}