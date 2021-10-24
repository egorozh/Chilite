using Chilite.Database;
using Chilite.DomainModels;
using Microsoft.Extensions.DependencyInjection;

namespace Chilite.Domain;

public class UserSession : IDisposable
{
    #region Private Fields

    private ChatApp _chatApp;

    #endregion

    #region Public Properties

    public string UserId { get; }

    #endregion

    #region Events

    public event Action<ChatMessage>? NewMessageSended;

    #endregion

    #region Constructor

    public UserSession(ChatApp chatApp, string userId)
    {
        UserId = userId;
        _chatApp = chatApp;
    }

    #endregion
    
    #region Public Methods

    public void Init()
    {
        using var scope = _chatApp.ServiceScopeFactory.CreateScope();

        var chatDbContext = scope.ServiceProvider.GetRequiredService<ChatDbContext>();

        foreach (var chatMessage in chatDbContext.Messages) 
            NewMessageSended?.Invoke(chatMessage);
    }

    public void Dispose()
    {
        _chatApp.Sessions.Remove(this);
        _chatApp = null!;
    }

    public void MessageInvoked(ChatMessage chatMessage)
    {
        NewMessageSended?.Invoke(chatMessage);
    }

    #endregion
}