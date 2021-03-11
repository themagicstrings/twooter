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
using static Api.TwooterOptions;

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

        private async Task CheckSessionForUser() {

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
                Content = BasicTemplater.GenerateTimeline(messages, timelineType.SELF, user)
            };
        }

        // Displays specific users messages
        [HttpGet("{username}")]
        public async Task<ActionResult> GetUserAsync(string username)
        {
            await CheckSessionForUser();

            var searchedUser = await UserRepo.ReadAsync(username, MessageLimit);

            if (searchedUser is null) return (ActionResult)await Get404Page();

            return new ContentResult
            {
                ContentType = "text/html",
                StatusCode = Status200OK,
                Content = BasicTemplater.GenerateTimeline(searchedUser.messages,timelineType.OTHER, user: user, otherPersonUsername: searchedUser.username)
            };
        }

        // Attempts to register a user with given information
        [HttpPost("/sign_up")]
        public async Task<IActionResult> CreateUserAsync([FromForm]UserCreateDTO user)
        {
            if(user.Username == "" || user.Username is null) return generateBadRequestRegister("You have to enter a username");
            if(user.Email is null || !user.Email.Contains('@')) return generateBadRequestRegister("You have to enter a valid email address");
            if(user.Password1 == "" || user.Password1 is null) return generateBadRequestRegister("You have to enter a password");
            if(user.Password1 != user.Password2) return generateBadRequestRegister("The two passwords do not match");

            var exist = await UserRepo.ReadAsync(user.Username);

            if(exist is not null)
            {
                return generateBadRequestRegister("The username is already taken");
            }
            // return BadRequest("The username is already taken");

            await UserRepo.CreateAsync(user);
            BasicTemplater.flashes.Add("You were successfully registered and can login now");
            return Redirect("/login");
            //return Ok("You were succesfully registered and can login now");
        }

        private static ContentResult generateBadRequestRegister(string message)
        {
            BasicTemplater.errors.Add(message);
                var toReturn = new ContentResult {
                    ContentType = "text/html",
                    StatusCode = Status400BadRequest,
                    Content = BasicTemplater.GenerateRegisterPage()
                };
            return toReturn;
        }

        private static ContentResult generateBadRequestLogin(string message)
        {
            BasicTemplater.errors.Add(message);
                var toReturn = new ContentResult {
                    ContentType = "text/html",
                    StatusCode = Status400BadRequest,
                    Content = BasicTemplater.GenerateLoginPage()
                };
            return toReturn;
        }

        // Displays register page
        [HttpGet("/sign_up")]
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
        }


        // Attemps to follow a user
        [HttpPost("/{username}/follow")]
        public async Task<IActionResult> FollowUserAsync([FromRoute] string username)
        {
            await CheckSessionForUser();

            var res = await UserRepo.FollowAsync(user.username, username);

            if (res != 0) return BadRequest();
            BasicTemplater.flashes.Add($@"You are now following ""{username}""");
            return Redirect($"/{username}");
        }

        // Attemps to unfollow a user
        [HttpPost("/{username}/unfollow")]
        public async Task<IActionResult> UnfollowUserAsync([FromRoute] string username)
        {
            await CheckSessionForUser();

            var res = await UserRepo.UnfollowAsync(user.username, username);

            if (res == -3) return (ActionResult)await Get404Page();
            if (res != 0) return BadRequest();
            BasicTemplater.flashes.Add($@"You are no longer following ""{username}""");
            return Redirect($"/{username}");
        }

        // Displays all messages
        [HttpGet("/public")]
        public async Task<IActionResult> GetPublicTimeline()
        {
            await CheckSessionForUser();

            return new ContentResult {
                ContentType = "text/html",
                StatusCode = (int) Status200OK,
                Content = BasicTemplater.GenerateTimeline(messages: await MessageRepo.ReadAllAsync(MessageLimit), timelineType.PUBLIC, user: user)
            };
        }

        // Attempts to login a user
        [HttpPost("/login")]
        public async Task<IActionResult> PostLogin([FromForm] UserLoginDTO loginDTO)
        {
            string hash;
            if (loginDTO.Username is null || (hash = await UserRepo.ReadPWHash(loginDTO.Username)) is null)
            {
                return generateBadRequestLogin("Invalid username");
            }

            if (loginDTO.Password is null)
            {
                return generateBadRequestLogin("Invalid password");
            }

            var hashedpwd = UserRepo.HashPassword(loginDTO.Password);

            if (hash is null || hashedpwd is null || !hash.Equals(hashedpwd))
            {
                return generateBadRequestLogin("Invalid password");
            }

            var user = await UserRepo.ReadAsync(loginDTO.Username);

            sessionHelper.SetString("user_id", user.user_id.ToString());
            BasicTemplater.flashes.Add("You were logged in");
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
            BasicTemplater.flashes.Add("You were logged out");
            HttpContext.Session.Clear();
            return Redirect("~/public");
        }

        [HttpGet("/404")]
        public async Task<IActionResult> Get404Page()
        {
            await CheckSessionForUser();
            return new ContentResult {
                ContentType = "text/html",
                StatusCode = (int) Status404NotFound,
                Content = BasicTemplater.Generate404Page(user)
            };
        }
    }
}
