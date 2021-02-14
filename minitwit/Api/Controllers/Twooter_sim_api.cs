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
    public class Twooter_sim_api :Controller
    {
        private readonly IMessageRepository MessageRepo;
        private readonly IUserRepository UserRepo;
        private readonly SessionHelper sessionHelper;
        private UserReadDTO user = null;
        public Twooter_sim_api(IMessageRepository msgrepo, IUserRepository usrrepo)
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
        [HttpGet("{username}")]
        public async Task<ActionResult> get_user_id(string username)
        {
            await CheckSessionForUser();

            var searchedUser = await UserRepo.ReadAsync(username);

            if (searchedUser is null) return NotFound();

            return new ContentResult 
            {
                ContentType = "int",
                StatusCode = Status200OK,
                Content = BasicTemplater.GenerateTimeline(searchedUser.messages, user != null)
            };
        }
        

        [HttpPost("/register")]
        public async Task<IActionResult> register([FromForm]UserCreateDTO user)
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
        [HttpGet("/msgs")]
        public async Task<IActionResult> messages()
        {
            return new ContentResult {
                ContentType = "text/html",
                StatusCode = (int) Status200OK,
                Content = BasicTemplater.GenerateTimeline(await MessageRepo.ReadAllAsync(), user != null )
            };
        }

        [Route("/msgs/<username>")]
        [AcceptVerbs("GET","POST")]
        public async Task<ActionResult> messages_per_user(string username)
        {
            await CheckSessionForUser();
            if(Request.Method == "GET"){
                var searchedUser = await UserRepo.ReadAsync(username);

                if (searchedUser is null) return NotFound();

                return new ContentResult 
                {
                    ContentType = "text/html",
                    StatusCode = Status200OK,
                    Content = BasicTemplater.GenerateTimeline(searchedUser.messages, user != null)
                };
            }
            else if(Request.Method == "POST")
            {
                var request_data = Request.Body;
                var query = """INSERT INTO messages
            }

        }
        [HttpPost("/logout")]
        public async Task<IActionResult> PostLogout()
        {
            HttpContext.Session.Clear();
            return Redirect("~/public");
        }
    }
}