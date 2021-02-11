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
            var newUser = new User
            {
                username = user.username,
                email = user.email,
                pw_hash = user.pw_hash
            };

            await context.users.AddAsync(newUser);
            await context.SaveChangesAsync();

            return newUser.user_id;
        }

        public async Task<int> DeleteAsync(int id)
        {
            var query =
                from user in context.users
                where user.user_id == id
                select user;
            // var query2 = context.users.Where(u => u.user_id == id).Select(u => u);
            if (!query.Any()) return -1;
            var foundUser = await query.FirstOrDefaultAsync();
            context.users.Remove(foundUser);
            await context.SaveChangesAsync();
            return foundUser.user_id;
        }

        public async Task<UserReadDTO> ReadAsync(int id)
        {
            var query = 
                from user in context.users
                where user.user_id == id
                select user;
            var foundUser = await query.FirstOrDefaultAsync();
            return new UserReadDTO
            {
                user_id = foundUser.user_id,
                username = foundUser.username,
                email = foundUser.email
            };


        }
    }
}