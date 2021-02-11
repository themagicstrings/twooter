using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Models;
using Shared;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class MessageController : ControllerBase
    {
        private readonly IUserRepository repository;
        
        public MessageController(IUserRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserReadDTO>> GetMessage(int id)
        {
            var message = await repository.ReadAsync(id);
            if (message == null) return NotFound();
            return message;
        }
    }
}