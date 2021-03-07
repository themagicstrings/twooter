using System.Threading.Tasks;
using System.Collections.Generic;
using Entities;
using Shared;

namespace Models
{
    public interface IMessageRepository
    {
        Task<MessageReadDTO> ReadAsync(int id);
        Task<int> CreateAsync(string message, string username);
        Task<int> DeleteAsync(int id);
        Task<List<MessageReadDTO>> ReadAllAsync(int noOfMessages = int.MaxValue);
    }
}