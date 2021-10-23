using Chilite.Database;
using Chilite.DomainModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Chilite.Domain;

public class ChatApp
{
    internal IServiceScopeFactory ServiceProvider;
    internal List<UserSessions> Sessions = new();

    public ChatApp(IServiceScopeFactory scopeFactory)
    {
        ServiceProvider = scopeFactory;
    }

    public UserSessions JoinUser(ChatUser user)
    {
        var userSession = new UserSessions(this, user);

        Sessions.Add(userSession);

        return userSession;
    }

    public async Task SendAsync(ChatUser user, ChatMessage chatMessage)
    {
        using var scope = ServiceProvider.CreateScope();

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
            LogException(ex);
        }
        finally
        {
            var old = Console.BackgroundColor;
            Console.BackgroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("finally");
            Console.BackgroundColor = old;
        }
    }

    void LogException(Exception error)
    {
        Exception realerror = error;
        while (realerror.InnerException != null)
            realerror = realerror.InnerException;

        var old = Console.BackgroundColor;
        Console.BackgroundColor = ConsoleColor.DarkMagenta;
        Console.WriteLine(realerror.ToString());
        Console.BackgroundColor = old;
    }
}