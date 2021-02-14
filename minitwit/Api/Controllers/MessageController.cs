using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Models;
using Shared;
using System.Collections.Generic;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Controllers
{
    [ApiController]
    [Route("/")]
    public class MessageController : ControllerBase
    {
        private readonly IMessageRepository repository;
        
        public MessageController(IMessageRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MessageReadDTO>> GetMessageAsync(int id)
        {
            var message = await repository.ReadAsync(id);
            if (message == null) return NotFound();
            return message;
        }

        [HttpGet("all_messages")]
        public async Task<ActionResult<List<MessageReadDTO>>> GetMessagesAsync()
        {
            return await repository.ReadAllAsync();
        }

        [HttpGet("public")]
        public async Task<ContentResult> PublicTimeline()
        {
            return new ContentResult {
                ContentType = "text/html",
                StatusCode = (int) Status200OK,
                Content = "<html><body>Hello World!</body></html>"
            };
        }

        [HttpPost("add_message")]
        public async Task<IActionResult> PostMessageAsync([FromBody] MessageCreateDTO message)
        {
            var id = await repository.CreateAsync(message);

            if (id == -1) return BadRequest();
            return CreatedAtAction(nameof(GetMessageAsync), new { id }, default);
        }
    }
}