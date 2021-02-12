using System.Threading.Tasks;
using Entities;
using Shared;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

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

        public async Task<int> FollowAsync(string follower, string followed )
        {
            var followedQuery = from u in context.users where u.username == followed select u;
            if(!await followedQuery.AnyAsync()) return -1;
            var followerQuery = from u in context.users where u.username == follower select u;
            if(!await followerQuery.AnyAsync()) return -2;

            var newFollow = new Follow
            {
                FollowedId = (await followerQuery.FirstOrDefaultAsync()).user_id,
                FollowerId = (await followedQuery.FirstOrDefaultAsync()).user_id
            };

            await context.follows.AddAsync(newFollow);
            await context.SaveChangesAsync();

            return 0;
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

        public async Task<UserReadDTO> ReadAsync(string username)
        {
            var query = from u in context.users where u.username.Equals(username) select
                new UserReadDTO { 
                    user_id = u.user_id,
                    username = u.username,
                    email = u.email,
                    followers = (u.FollowedBy.Select(f => f.Followed.username)).ToList(),
                    following = (u.Following.Select(f => f.Follower.username)).ToList(),
                    messages = (from m in u.Messages select new MessageReadDTO{
                        id = m.message_id,
                        text = m.text,
                        pub_date = m.pub_date,
                        flagged = m.flagged
                    }).ToList()
                };
            
            return await query.FirstOrDefaultAsync();
        }

        public async Task<List<UserReadDTO>> ReadAllAsync()
        {
            var query = from u in context.users select
                new UserReadDTO { 
                    user_id = u.user_id,
                    username = u.username,
                    email = u.email,
                    followers = (u.FollowedBy.Select(f => f.Followed.username)).ToList(),
                    following = (u.Following.Select(f => f.Follower.username)).ToList(),
                    messages = (from m in u.Messages select new MessageReadDTO{
                        id = m.message_id,
                        text = m.text,
                        pub_date = m.pub_date,
                        flagged = m.flagged
                    }).ToList()
                };
            
            return await query.ToListAsync();
        }
    }
}