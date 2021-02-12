using System.Threading.Tasks;
using Entities;
using Shared;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System;

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
                pub_date = DateTime.Now,
                flagged = 0
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
                select new MessageReadDTO {
                    id = message.message_id,
                    text = message.text,
                    pub_date = message.pub_date,
                    flagged = message.flagged,
                    author = (from user in context.users 
                             where user.user_id == message.author_id 
                             select new UserReadDTO { 
                                 user_id = user.user_id,
                                 username = user.username,
                                 email = user.email
                             }).FirstOrDefault()
                };

            return await query.FirstOrDefaultAsync();
        }

        public async Task<List<MessageReadDTO>> ReadAllAsync()
        {
            var query =
                from message in context.messages
                select new MessageReadDTO {
                    id = message.message_id,
                    text = message.text,
                    pub_date = message.pub_date,
                    flagged = message.flagged,
                    author = (from user in context.users 
                             where user.user_id == message.author_id 
                             select new UserReadDTO { 
                                 user_id = user.user_id,
                                 username = user.username,
                                 email = user.email
                             }).FirstOrDefault()
                };

            return await query.ToListAsync();
        }
    }
}