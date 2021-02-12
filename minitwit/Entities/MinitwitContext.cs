using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;
using Microsoft.EntityFrameworkCore.SqlServer;

namespace Entities
{
    public class MinitwitContext : DbContext, IMinitwitContext
    {
        public DbSet<User> users { get; set; }

        public DbSet<Message> messages { get; set; }

        public DbSet<Follower> followers { get; set; }

        public MinitwitContext() {}

        public MinitwitContext(DbContextOptions<MinitwitContext> options)
        : base(options) {}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if(!optionsBuilder.IsConfigured)
            {
                var connectionString = @"Server=localhost;Database=twooter;Trusted_Connection=True";
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Follower>()
                .HasKey(f => new {f.who_id, f.whom_id});

            modelBuilder.Entity<User>()
                .HasIndex(u => u.username)
                .IsUnique();
            
            modelBuilder.Entity<User>()
                .HasIndex(u => u.email)
                .IsUnique();
        }

    }
}