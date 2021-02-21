using System;
using System.Threading.Tasks;
using Entities;
using Shared;
using System.Collections.Generic;

namespace Models
{
    public interface IUserRepository
    {
        Task<UserReadDTO> ReadAsync(string name);
        Task<string> ReadPWHash(string name);
        Task<UserReadDTO> ReadAsync(int id);
        Task<List<UserReadDTO>> ReadAllAsync();
        Task<int> CreateAsync(UserCreateDTO user);
        Task<int> DeleteAsync(int id);
        Task<int> FollowAsync(string follower, string followed);
        Task<int> UnfollowAsync(string follower, string followed);
        string HashPassword(string password);
    }
}
