using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Chilite.Database
{
    public class ChatDbContext : IdentityDbContext<ChatUser>
    {
        public DbSet<ChatMessage> Messages { get; set; }

        public ChatDbContext()
        {
        }

        public ChatDbContext(DbContextOptions<ChatDbContext> options) : base(options)
        {
        }
    }
}