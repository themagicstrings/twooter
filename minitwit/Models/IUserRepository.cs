using System;
using System.Threading.Tasks;
using Entities;
using Shared;

namespace Models
{
    public interface IUserRepository
    {
        Task<UserReadDTO> ReadAsync(string name);
        Task<int> CreateAsync(UserCreateDTO user);
        Task<int> DeleteAsync(int id);
    }
}
