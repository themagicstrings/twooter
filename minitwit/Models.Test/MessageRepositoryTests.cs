using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using System.Threading.Tasks;
using Entities;
using Shared;
using Xunit;

namespace Models.Test
{
    public class MessageRepositoryTests : IDisposable
    {
        private readonly MinitwitContext context;
        private readonly MessageRepository repo;

        public MessageRepositoryTests()
        {
            var builder = new DbContextOptionsBuilder<MinitwitContext>().UseInMemoryDatabase("minitwittest");
            context = new MinitwitTestContext(builder.Options);
            context.Database.EnsureCreated();
            repo = new MessageRepository(context);
        }

        [Fact]
        public async Task create_and_read_message_existing_user()
        {
            var result = await repo.CreateAsync("testtext", "olduser1");
            var message = await repo.ReadAsync(result);

            Assert.NotNull(message);
            Assert.Equal("testtext", message.text);
            Assert.Equal("olduser1", message.author.username);
        }

        [Fact]
        public async Task create_message_non_existing_user()
        {
            var result = await repo.CreateAsync("testtext", "nonuser");

            Assert.Equal(-1, result);
        }

        [Fact]
        public async Task read_nonexisting_message()
        {
            var result = await repo.ReadAsync(1337);

            Assert.Null(result);
        }

        [Fact]
        public async Task delete_existing_message()
        {
            var messageId = await repo.CreateAsync("testtext", "olduser2");
            var messagebefore = await repo.ReadAsync(messageId);
            var result = await repo.DeleteAsync(messageId);
            var messageafter = await repo.ReadAsync(messageId);

            Assert.NotNull(messagebefore);
            Assert.Null(messageafter);
            Assert.Equal(messageId, result);
        }

        [Fact]
        public async Task delete_nonexisting_message()
        {
            var result = await repo.DeleteAsync(1337);

            Assert.Equal(-1, result);
        }

        [Fact]
        public async Task readall_messages()
        {
            await repo.CreateAsync("testtext1","olduser1");
            var first = await repo.ReadAllAsync();
            await repo.CreateAsync("testtext2","olduser1");
            var second = await repo.ReadAllAsync();

            Assert.Equal(2, first.Count);
            Assert.Equal(3, second.Count);
        }

        public void Dispose()
        {
            context.Dispose();
        }
    }
}