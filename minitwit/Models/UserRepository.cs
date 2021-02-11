using System.Threading.Tasks;
using Entities;
using Shared;

namespace Models
{
    public class UserRepository : IUserRepository
    {
        private readonly IMinitwitContext context;

        public UserRepository(IMinitwitContext context)
        {
        this.context = context;
        }
        public Task<int> CreateAsync(UserCreateDTO user)
        {
            throw new System.NotImplementedException();
        }

        public Task<int> DeleteAsync(int id)
        {
            throw new System.NotImplementedException();
        }

        public Task<User> ReadAsync(int id)
        {
            throw new System.NotImplementedException();
        }
    }
}