using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;

namespace Entities
{
    public class MinitwitContext : DbContext, IMinitwitContext
    {
        public DbSet<User> users { get; set; }

        public DbSet<Message> messages { get; set; }

        public MinitwitContext() {}

        public MinitwitContext(DbContextOptions<MinitwitContext> options)
        : base(options) {}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if(!optionsBuilder.IsConfigured)
            {
                var connectionString = @"Filename=:memory:";
                optionsBuilder.UseSqlite(connectionString);
            }
        }

    }
}