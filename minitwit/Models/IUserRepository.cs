﻿using System;
using System.Threading.Tasks;
using Entities;
using Shared;
using System.Collections.Generic;

namespace Models
{
    public interface IUserRepository
    {
        Task<UserReadDTO> ReadAsync(string name);
        Task<List<UserReadDTO>> ReadAllAsync();
        Task<int> CreateAsync(UserCreateDTO user);
        Task<int> DeleteAsync(int id);
        Task<int> FollowAsync(string follower, string followed);
    }
}