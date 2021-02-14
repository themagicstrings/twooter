using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;
using Microsoft.EntityFrameworkCore.SqlServer;

namespace Entities
{
    public class MinitwitContext : DbContext, IMinitwitContext
    {
        public DbSet<User> users { get; set; }

        public DbSet<Message> messages { get; set; }

        public DbSet<Follow> follows { get; set; }

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
            modelBuilder.Entity<User>()
                .HasIndex(u => u.username)
                .IsUnique();
            
            modelBuilder.Entity<User>()
                .HasIndex(u => u.email)
                .IsUnique();
            
            modelBuilder.Entity<User>()
                .HasMany(u => u.Messages)
                .WithOne(m => m.User);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.User)
                .WithMany(u => u.Messages);

            modelBuilder.Entity<Follow>()
                .HasKey(f => new {f.FollowerId, f.FollowedId});

            modelBuilder.Entity<Follow>()
                .HasOne(pt => pt.Followed)
                .WithMany(p => p.FollowedBy)
                .HasForeignKey(pt => pt.FollowedId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Follow>()
                .HasOne(pt => pt.Follower)
                .WithMany(p => p.Following)
                .HasForeignKey(pt => pt.FollowerId);
        }
    }
}