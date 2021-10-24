using Chilite.Database;
using Chilite.DomainModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Chilite.Domain;

public class ChatApp
{
    internal IServiceScopeFactory ServiceScopeFactory;
    internal ILogger<ChatApp> Logger;
    internal List<UserSession> Sessions = new();

    public ChatApp(IServiceScopeFactory scopeFactory, ILogger<ChatApp> logger)
    {
        ServiceScopeFactory = scopeFactory;
        Logger = logger;
    }

    public UserSession JoinUser(ChatUser user)
    {
        var userSession = new UserSession(this, user);

        Sessions.Add(userSession);

        return userSession;
    }

    public async Task SendAsync(ChatUser user, ChatMessage chatMessage)
    {
        using var scope = ServiceScopeFactory.CreateScope();

        var chatDbContext = scope.ServiceProvider.GetRequiredService<ChatDbContext>();

        foreach (var session in Sessions)
        {
            session.MessageInvoked(chatMessage);
        }

        try
        {
            var userDb = await chatDbContext.Users.FirstAsync(u => u.Id == user.Id);

            chatMessage.User = userDb;

            await chatDbContext.Messages.AddAsync(chatMessage);
            await chatDbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            LogException(ex, "SendAsync");
        }
    }

    private void LogException(Exception error, string message)
    {
        var realerror = error;

        while (realerror.InnerException != null)
            realerror = realerror.InnerException;

        Logger.LogError(error, $"ChatApp.{message}");
    }
}