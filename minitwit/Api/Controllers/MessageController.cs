using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Models;
using Shared;
using System.Collections.Generic;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class MessageController : ControllerBase
    {
        private readonly IMessageRepository repository;
        
        public MessageController(IMessageRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MessageReadDTO>> GetMessage(int id)
        {
            var message = await repository.ReadAsync(id);
            if (message == null) return NotFound();
            return message;
        }

        [HttpGet]
        public async Task<ActionResult<List<MessageReadDTO>>> GetMessage()
        {
            var messages = await repository.ReadAllAsync();
            if (messages is null) return NotFound();
            return messages;
        }
    }
}