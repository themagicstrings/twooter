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

        [HttpGet("{id}")]
        public async Task<ActionResult<UserReadDTO>> GetUser(int id) {
            var user = await repository.ReadAsync(id);
            if (user == null) return NotFound();
            return user;
        }
    }
}