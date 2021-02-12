using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Entities
{
    public interface IMinitwitContext
    {
        public DbSet<User> users { get; }
        public DbSet<Message> messages { get; }
        public DbSet<Follower> followers { get; }
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}