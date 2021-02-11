using System;
using System.Threading.Tasks;
using Entities;
using Shared;

namespace Models
{
    public interface IUserRepository
    {
        Task<User> ReadAsync(int id);
        Task<int> CreateAsync(UserCreateDTO user);
        Task<int> DeleteAsync(int id);
        
    }
}
