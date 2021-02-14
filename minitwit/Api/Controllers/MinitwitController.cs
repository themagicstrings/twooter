using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Models;
using Shared;
using System.Collections.Generic;
using static Microsoft.AspNetCore.Http.StatusCodes;

using System;

namespace Controllers
{
    [ApiController]
    [Route("/")]
    public class MinitwitController : ControllerBase
    {
        private readonly IMessageRepository MessageRepo;
        private readonly IUserRepository UserRepo;

        public MinitwitController(IMessageRepository msgrepo, IUserRepository usrrepo)
        {
            this.MessageRepo = msgrepo;
            this.UserRepo = usrrepo;
        }

        // Redirects to /public if no user is logged in, else displays users timeline including followed users
        [HttpGet]
        public async Task<IActionResult> GetRoot()
        {
            throw new NotImplementedException();
        }

        // Displays specific users messages
        [HttpGet("{username}")]
        public async Task<ActionResult> GetUserAsync(string username)
        {
            throw new NotImplementedException();
        }

        // Attempts to register a user with given information
        [HttpPost("/register")]
        public async Task<IActionResult> CreateUserAsync(UserCreateDTO user)
        {
            if(user.username == "" || user.username is null) return BadRequest("You have to enter a username");
            if(!user.email.Contains('@')) return BadRequest("You have to enter a valid email address");
            if(user.password1 == "" || user.password1 is null) return BadRequest("You have to enter a password");
            if(user.password1 != user.password2) return BadRequest("The two passwords do not match");

            var exist = await UserRepo.ReadAsync(user.username);

            if(exist is not null) return BadRequest("The username is already taken");

            await UserRepo.CreateAsync(user);
            return Ok("You were succesfully registered and can login now");
        }
        
        // Displays register page
        [HttpGet("/register")]
        public async Task<IActionResult> GetRegisterPage()
        {
            throw new NotImplementedException();
        }


        // Attemps to follow a user
        [HttpPost("{followed}/follow")]
        public async Task<IActionResult> FollowUserAsync([FromBody] string follower, string followed)
        {
            var res = await UserRepo.FollowAsync(followed, follower);

            if (res != 0) return BadRequest();
            return Ok();
        }

        // Attemps to unfollow a user
        [HttpDelete("{unfollowed}/unfollow")]
        public async Task<IActionResult> UnfollowUserAsync([FromBody] string unfollower, string unfollowed)
        {
            var res = await UserRepo.UnfollowAsync(unfollowed, unfollower);

            if (res == -3) return NotFound();
            if (res != 0) return BadRequest();
            return Ok();
        }

        // Displays all messages
        [HttpGet("/public")]
        public async Task<IActionResult> GetPublicTimeline()
        {
            throw new NotImplementedException();
        }

        // Attempts to login a user
        [HttpPost("/login")]
        public async Task<IActionResult> PostLogin()
        {
            throw new NotImplementedException();
        }

        // Displays login page
        [HttpGet("/login")]
        public async Task<IActionResult> GetLoginPage()
        {
            throw new NotImplementedException();
        }

        // Logs out currently logged in user
        [HttpPost("/logout")]
        public async Task<IActionResult> PostLogout()
        {
            throw new NotImplementedException();
        }
    }
}