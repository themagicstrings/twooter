using System.Threading.Tasks;
using Entities;
using Shared;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Models
{
    public class UserRepository : IUserRepository
    {
        private readonly IMinitwitContext context;

        public UserRepository(IMinitwitContext context)
        {
            this.context = context;
        }
        public async Task<int> CreateAsync(UserCreateDTO user)
        {
            var User = new User {
                username = user.username,
                email = user.email,
                pw_hash = user.pw_hash
            };

            await context.users.AddAsync(User);
            await context.SaveChangesAsync();

            return 0;
        }

        public Task<int> DeleteAsync(int id)
        {
            throw new System.NotImplementedException();
        }

        public async Task<UserReadDTO> ReadAsync(string username)
        {
            var query = from u in context.users where u.username.Equals(username) select
                new UserReadDTO { 
                    username = u.username,
                    email = u.email
                };
            
            return await query.FirstOrDefaultAsync();
        }
    }
}