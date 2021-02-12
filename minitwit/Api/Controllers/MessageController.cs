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
        public async Task<ActionResult<List<MessageReadDTO>>> GetMessages()
        {
            return await repository.ReadAllAsync();
        }

        [HttpPost]
        public async Task<IActionResult> PostMessage([FromBody] MessageCreateDTO message)
        {
            var id = await repository.CreateAsync(message);

            if (id == -1) return BadRequest();
            return CreatedAtAction(nameof(GetMessage), new { id }, default);
        }
    }
}