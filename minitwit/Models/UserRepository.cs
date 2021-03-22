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
            if(user.Password1 == "" && user.Password2 == "") return -1;

            if(!user.Email.Contains("@")) return -2;

            if(!user.Password1.Equals(user.Password2)) return -3;

            var usernamecheck = from u in context.users where u.username == user.Username select u;
            if(await usernamecheck.AnyAsync()) return -4;

            var emailcheck = from u in context.users where u.email == user.Email select u;
            if(await emailcheck.AnyAsync()) return -5;

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
            //Check if following self
            if (follower.Equals(followed)) return -1;

            //Check if refered users exists
            var userQuery = from u in context.users where u.username == followed || u.username == follower select u;
            if((await userQuery.CountAsync()) != 2) return -2;

            //Check if follow already exists
            var relationExists = from entry in context.follows
                where entry.Followed.username == followed &&
                      entry.Follower.username == follower
                select entry;
            if (await relationExists.AnyAsync()) return -3;

            var newFollow = new Follow
            {
                FollowedId = await userQuery.Where(u => u.username == followed).Select(u => u.user_id).FirstAsync(),
                FollowerId = await userQuery.Where(u => u.username == follower).Select(u => u.user_id).FirstAsync()
            };

            await context.follows.AddAsync(newFollow);
            await context.SaveChangesAsync();

            return 0;
        }

        public async Task<int> UnfollowAsync(string unfollower, string unfollowed)
        {
            if (unfollower.Equals(unfollowed)) return -1;

            var followEntity = await context.follows.Where(f => f.Followed.username == unfollowed && f.Follower.username == unfollower).Select(f => f).FirstOrDefaultAsync();
            if (followEntity is null) return -2;

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

        public async Task<UserReadDTO> ReadAsync(string username, int noOfMessages = int.MaxValue)
        {
            var query = from u in context.users where u.username.Equals(username) select
                new UserReadDTO {
                    user_id = u.user_id,
                    username = u.username,
                    email = u.email,
                    followers = (context.follows.Where(f => f.Followed.username == username).Select(f => f.Follower.username).ToList()),
                    following = (context.follows.Where(f => f.Follower.username == username).Select(f => f.Followed.username).ToList()),
                    messages = (
                        from m in u.Messages
                        orderby m.pub_date descending
                        select new MessageReadDTO{
                            author = new UserReadDTO {username = u.username, email = u.email },
                            id = m.message_id,
                            text = m.text,
                            pub_date = m.pub_date,
                            flagged = m.flagged
                        }
                    ).Take(noOfMessages).ToList()
                };

            return await query.FirstOrDefaultAsync();
        }

        public async Task<List<UserReadDTO>> ReadAllAsync(int noOfMessages = int.MaxValue)
        {
            var query = from u in context.users select
                new UserReadDTO {
                    user_id = u.user_id,
                    username = u.username,
                    email = u.email,
                    followers = (u.FollowedBy.Select(f => f.Followed.username)).ToList(),
                    following = (u.Following.Select(f => f.Follower.username)).ToList(),
                    messages = (
                        from m in u.Messages
                        orderby m.pub_date descending
                        select new MessageReadDTO{
                        id = m.message_id,
                        text = m.text,
                        pub_date = m.pub_date,
                        flagged = m.flagged
                    }).Take(noOfMessages).ToList()
                };

            return await query.ToListAsync();
        }
    public int GetTotalUsers()
    {
        var query = (from u in context.users select  u.user_id).Count();
        return query;    
    }
    public async Task<UserReadDTO> ReadAsync(int id, int noOfMessages = int.MaxValue)
    {
        var query = from u in context.users where u.user_id.Equals(id) select
            new UserReadDTO {
                user_id = u.user_id,
                username = u.username,
                email = u.email,
                followers = (context.follows.Where(f => f.Followed.username == u.username).Select(f => f.Follower.username).ToList()),
                following = (context.follows.Where(f => f.Follower.username == u.username).Select(f => f.Followed.username).ToList()),
                messages = (
                    from m in u.Messages
                    orderby m.pub_date descending
                    select new MessageReadDTO{
                        author = new UserReadDTO {username = u.username, email = u.email},
                        id = m.message_id,
                        text = m.text,
                        pub_date = m.pub_date,
                        flagged = m.flagged
                    }
                ).Take(noOfMessages).ToList()
            };

        return await query.FirstOrDefaultAsync();
    }

    public async Task<string> ReadPWHash(string name)
    {
      return await (from u in context.users where u.username.Equals(name) select u.pw_hash).FirstOrDefaultAsync();
    }
  }
}
