using System.Threading.Tasks;
using Entities;
using Shared;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Models
{
    public class MessageRepository : IMessageRepository
    {
        private readonly IMinitwitContext context;

        public MessageRepository(IMinitwitContext context)
        {
            this.context = context;
        }
        public async Task<int> CreateAsync(MessageCreateDTO message)
        {
            var newMessage = new Message
            {
                author_id = message.user.user_id,
                text = message.text,
                pub_date = message.pub_date,
                flagged = message.flagged
            };
            await context.messages.AddAsync(newMessage);
            await context.SaveChangesAsync();
            return newMessage.message_id;
        }

        public async Task<int> DeleteAsync(int id)
        {
            var query =
                from message in context.messages
                where message.message_id == id
                select message;
            var foundMessage = await query.FirstOrDefaultAsync();
            context.messages.Remove(foundMessage);
            await context.SaveChangesAsync();
            return foundMessage.message_id;

        }

        public async Task<MessageReadDTO> ReadAsync(int id)
        {
            var query =
                from message in context.messages
                where message.message_id == id
                select message;
            var foundMessage = await query.FirstOrDefaultAsync();
            return new MessageReadDTO
            {
                message_id = foundMessage.message_id,
                author_id = foundMessage.author_id,
                text = foundMessage.text,
                pub_date = foundMessage.pub_date,
                flagged = foundMessage.flagged
            };
        }
    }
}