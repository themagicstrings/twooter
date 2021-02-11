using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Models;
using Shared;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository repository;
        
        public UserController(IUserRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<UserReadDTO>> GetUserAsync(string username) 
        {
            return await repository.ReadAsync(username);
        }

        [HttpPost]
        public async Task<IActionResult> PostUserAsync([FromBody] UserCreateDTO user)
        {
            var res = await repository.CreateAsync(user);

            return CreatedAtAction(nameof(GetUserAsync), new { res }, default);
        }
    }
}