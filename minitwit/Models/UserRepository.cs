using System.Threading.Tasks;
using System.Text;
using Entities;
using Shared;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Security.Cryptography;

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
                username = user.Username,
                email = user.Email,
                pw_hash = HashPassword(user.Password1)
            };

            await context.users.AddAsync(newUser);
            await context.SaveChangesAsync();

            return newUser.user_id;
        }

        public string HashPassword(string password)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));

                StringBuilder sb = new StringBuilder();
                foreach(byte b in bytes) sb.Append(b.ToString());
                return sb.ToString();
            }
        }

        public async Task<int> FollowAsync(string follower, string followed)
        {
            if (followed.Equals(followed)) return -1;
            var followedQuery = from u in context.users where u.username == followed select u;
            if(!await followedQuery.AnyAsync()) return -1;
            var followerQuery = from u in context.users where u.username == follower select u;
            if(!await followerQuery.AnyAsync()) return -2;

            var relationExists = from entry in context.follows
                where entry.Followed.username == followed &&
                      entry.Follower.username == follower
                select entry;

            if (await relationExists.AnyAsync()) return -3;

            var newFollow = new Follow
            {
                FollowedId = (await followedQuery.FirstOrDefaultAsync()).user_id,
                FollowerId = (await followerQuery.FirstOrDefaultAsync()).user_id
            };

            await context.follows.AddAsync(newFollow);
            await context.SaveChangesAsync();

            return 0;
        }

        public async Task<int> UnfollowAsync(string unfollower, string unfollowed)
        {
            var unfollowedQuery = from u in context.users where u.username == unfollowed select u;
            if(!await unfollowedQuery.AnyAsync()) return -1;
            var unfollowerQuery = from u in context.users where u.username == unfollower select u;
            if(!await unfollowerQuery.AnyAsync()) return -2;

            var followedUser = await unfollowedQuery.FirstAsync();
            var followerUser = await unfollowerQuery.FirstAsync();

            var followQuery = from f in context.follows where f.FollowedId == followedUser.user_id && f.FollowerId == followerUser.user_id select f;
            if(!await followQuery.AnyAsync()) return -3;

            var followEntity = await followQuery.FirstAsync();
            context.follows.Remove(followEntity);
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
                    followers = (context.follows.Where(f => f.Followed.username == username).Select(f => f.Follower.username).ToList()),//followers = (u.FollowedBy.Select(f => f.Followed.username)).ToList(),
                    following = (context.follows.Where(f => f.Follower.username == username).Select(f => f.Followed.username).ToList()),//following = (u.Following.Select(f => f.Follower.username)).ToList(),
                    messages = (from m in u.Messages select new MessageReadDTO{
                        author = new UserReadDTO {username = u.username},
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
        public async Task<List<FollowerListDTO>> ReadFollowerNameAsync()
        {
            var query = from u in context.users select
                new FollowerListDTO {
                    FollowerNameList = (u.FollowedBy.Select(f => f.Followed.username)).ToList()
                };

            return await query.ToListAsync();
        }

    public async Task<UserReadDTO> ReadAsync(int id)
    {
        var query = from u in context.users where u.user_id.Equals(id) select
            new UserReadDTO {
                user_id = u.user_id,
                username = u.username,
                email = u.email,
                followers = (context.follows.Where(f => f.Followed.username == u.username).Select(f => f.Follower.username).ToList()),//followers = (u.FollowedBy.Select(f => f.Followed.username)).ToList(),
                following = (context.follows.Where(f => f.Follower.username == u.username).Select(f => f.Followed.username).ToList()),
                messages = (from m in u.Messages select new MessageReadDTO{
                    author = new UserReadDTO {username = u.username},
                    id = m.message_id,
                    text = m.text,
                    pub_date = m.pub_date,
                    flagged = m.flagged
                }).ToList()
            };

        return await query.FirstOrDefaultAsync();
    }

    public async Task<string> ReadPWHash(string name)
    {
      return await (from u in context.users where u.username.Equals(name) select u.pw_hash).FirstOrDefaultAsync();
    }
  }
}
