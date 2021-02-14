using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Models;
using Shared;
using System.Collections.Generic;
using static Microsoft.AspNetCore.Http.StatusCodes;
using System.Web;
using System;
using Api;

namespace Controllers
{
    [ApiController]
    [Route("/")]
    public class MinitwitController : ControllerBase
    {
        private readonly IMessageRepository MessageRepo;
        private readonly IUserRepository UserRepo;
        private readonly SessionHelper sessionHelper;
        private UserReadDTO user = null;

        public MinitwitController(IMessageRepository msgrepo, IUserRepository usrrepo)
        {
            this.MessageRepo = msgrepo;
            this.UserRepo = usrrepo;
            this.sessionHelper = new SessionHelper(() => HttpContext.Session);
        }

        public async Task CheckSessionForUser() {

            if (int.TryParse(sessionHelper.GetString("user_id"), out var userid)) {
                var user = await UserRepo.ReadAsync(userid);
                if (user is null) await PostLogout();
                else this.user = user;
            }
        }

        // Redirects to /public if no user is logged in, else displays users timeline including followed users
        [HttpGet]
        public async Task<IActionResult> GetTimeline()
        {
            await CheckSessionForUser();

            if(user is null) return Redirect("/public");
            
            List<MessageReadDTO> messages = user.messages;
            foreach(string follow in user.following)
            {
                messages.AddRange((await UserRepo.ReadAsync(follow)).messages);
            }
            return new ContentResult 
            {
                ContentType = "text/html",
                StatusCode = Status200OK,
                Content = BasicTemplater.GenerateTimeline(messages, user != null)
            };
        }

        // Displays specific users messages
        [HttpGet("{username}")]
        public async Task<ActionResult> GetUserAsync(string username)
        {
            await CheckSessionForUser();

            return new ContentResult 
            {
                ContentType = "text/html",
                StatusCode = Status200OK,
                Content = BasicTemplater.GenerateTimeline(user.messages, user != null)
            };
        }

        // Attempts to register a user with given information
        [HttpPost("/register")]
        public async Task<IActionResult> CreateUserAsync([FromForm]UserCreateDTO user)
        {
            if(user.Username == "" || user.Username is null) return BadRequest("You have to enter a username");
            if(!user.Email.Contains('@')) return BadRequest("You have to enter a valid email address");
            if(user.Password1 == "" || user.Password1 is null) return BadRequest("You have to enter a password");
            if(user.Password1 != user.Password2) return BadRequest("The two passwords do not match");

            var exist = await UserRepo.ReadAsync(user.Username);

            if(exist is not null) return BadRequest("The username is already taken");

            await UserRepo.CreateAsync(user);
            return Redirect("/login");
            //return Ok("You were succesfully registered and can login now");
        }

        // Displays register page
        [HttpGet("/register")]
        public async Task<IActionResult> GetRegisterPage()
        {
            return new ContentResult {
                ContentType = "text/html",
                StatusCode = (int) Status200OK,
                Content = BasicTemplater.GenerateRegisterPage()
            };
        }

        // Post a message
        [HttpPost("add_message")]
        public async Task<IActionResult> PostMessageAsync([FromForm] MessageCreateRequestDTO message)
        {
            await CheckSessionForUser();

            var id = await MessageRepo.CreateAsync(message.Text, user.username);

            if (id == -1) return BadRequest();
            return Redirect("/");
            //return Ok("Your message was recorded");
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
            await CheckSessionForUser();

            return new ContentResult {
                ContentType = "text/html",
                StatusCode = (int) Status200OK,
                Content = BasicTemplater.GenerateTimeline(await MessageRepo.ReadAllAsync(), user != null )
            };
        }

        // Attempts to login a user
        [HttpPost("/login")]
        public async Task<IActionResult> PostLogin([FromForm] UserLoginDTO loginDTO)
        {
            if (loginDTO.Username is null || loginDTO.Password is null)
            {
                return BadRequest();
            }

            var hash = await UserRepo.ReadPWHash(loginDTO.Username);

            var hashedpwd = UserRepo.HashPassword(loginDTO.Password);

            if (!hash.Equals(hashedpwd))
            {
                return BadRequest();
            }

            var user = await UserRepo.ReadAsync(loginDTO.Username);

            sessionHelper.SetString("user_id", user.user_id.ToString());
            return Redirect("/");
        }

        // Displays login page
        [HttpGet("/login")]
        public async Task<IActionResult> GetLoginPage()
        {

            await CheckSessionForUser();

            if (user == null) {

            return new ContentResult(){
                Content = BasicTemplater.GenerateLoginPage(),
                StatusCode = (int) Status200OK,
                ContentType = "text/html"
            };}

            return new ContentResult(){
                Content = $"User: {user.username}"
            };
        }

        // Logs out currently logged in user
        [HttpPost("/logout")]
        public async Task<IActionResult> PostLogout()
        {
            HttpContext.Session.Clear();
            return Redirect("~/public");
        }
    }
}
