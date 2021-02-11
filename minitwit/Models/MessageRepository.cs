using System.Threading.Tasks;
using Entities;
using Shared;

namespace Models
{
    public class MessageRepository : IMessageRepository
    {
        private readonly IMinitwitContext context;

        public MessageRepository(IMinitwitContext context)
        {
            this.context = context;
        }
        public Task<int> CreateAsync(MessageCreateDTO message)
        {
            throw new System.NotImplementedException();
        }

        public Task<int> DeleteAsync(int id)
        {
            throw new System.NotImplementedException();
        }

        public Task<Message> ReadAsync(int id)
        {
            throw new System.NotImplementedException();
        }
    }
}