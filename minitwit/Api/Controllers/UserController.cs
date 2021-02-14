using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Models;
using Shared;
using static Microsoft.AspNetCore.Http.StatusCodes;
using System.Collections.Generic;

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

        [HttpGet]
        public async Task<ActionResult<List<UserReadDTO>>> GetUsersAsync()
        {
            return await repository.ReadAllAsync();
        }

        [HttpPost]
        public async Task<IActionResult> PostUserAsync([FromBody] UserCreateDTO user)
        {
            var res = await repository.CreateAsync(user);

            return CreatedAtAction(nameof(GetUserAsync), new { res }, default);
        }

        [HttpPost("follow/{followed}")]
        public async Task<IActionResult> FollowUserAsync([FromBody] string follower, string followed)
        {
            var res = await repository.FollowAsync(followed, follower);

            if (res != 0) return BadRequest();
            return Ok();
        }

        [HttpDelete("unfollow/{unfollowed}")]
        public async Task<IActionResult> UnfollowUserAsync([FromBody] string unfollower, string unfollowed)
        {
            var res = await repository.UnfollowAsync(unfollowed, unfollower);

            if (res == -3) return NotFound();
            if (res != 0) return BadRequest();
            return Ok();
        }
    }
}