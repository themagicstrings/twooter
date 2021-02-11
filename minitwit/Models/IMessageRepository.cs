using System.Threading.Tasks;
using System.Collections.Generic;
using Entities;
using Shared;

namespace Models
{
    public interface IMessageRepository
    {
        Task<MessageReadDTO> ReadAsync(int id);
        Task<int> CreateAsync(MessageCreateDTO message);
        Task<int> DeleteAsync(int id);
        Task<List<MessageReadDTO>> ReadAllAsync();
    }
}