using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Models;
using Shared;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using static Microsoft.AspNetCore.Http.StatusCodes;
using System.Web;
using System.Linq;
using System;
using Api;
using System.Text.Json;
using System.IO;
using System.Text.RegularExpressions;
using static Api.TwooterOptions;

namespace Controllers
{
    public class SimulationController : Controller
    {
        private readonly IMessageRepository MessageRepo;
        private readonly IUserRepository UserRepo;
        private readonly SessionHelper sessionHelper;
        private UserReadDTO user = null;


        private readonly string LATEST = "./LATEST.txt";


        public SimulationController(IMessageRepository msgrepo, IUserRepository usrrepo)
        {
            this.MessageRepo = msgrepo;
            this.UserRepo = usrrepo;
            this.sessionHelper = new SessionHelper(() => HttpContext.Session);
        }

        private async Task<int> get_user_id(string username)
        {
            var searchedUser = await UserRepo.ReadAsync(username);

            if (searchedUser is null) return -1;
            return searchedUser.user_id;
        }

        private int get_param_int(string name, int defaultValue)
        {
            Microsoft.Extensions.Primitives.StringValues query;
            if(Request.Query.TryGetValue(name, out query))
            {
                int val;
                if(int.TryParse(query.First(), out val)) return val;
                else throw new Exception("param " + name + " is not an int");
            }
            return defaultValue;
        }

        private async Task write_latest()
        {
            try {
                int val = get_param_int("latest", 100);
                await System.IO.File.WriteAllTextAsync(LATEST, "" + val);
            } catch {
                //ignore
            }
        }

        private async Task<int> read_latest()
        {
            try {
                string text = await System.IO.File.ReadAllTextAsync(LATEST);
                return int.Parse(text);
            } catch {
                await System.IO.File.WriteAllTextAsync(LATEST, "" + 0);
                return 0;
            }
        }

        [HttpGet("/latest")]
        public async Task<IActionResult> get_latest()
        {
            return new ContentResult{
                ContentType = "text/json",
                StatusCode = Status200OK,
                Content = JsonSerializer.Serialize(new {
                    latest = await read_latest()
                })
            };
        }

        [HttpPost("/register")]
        public async Task<IActionResult> register([FromBody] SimulationUserCreateDTO user)
        {
            await write_latest();

            if(user.Username == "" || user.Username is null) return BadRequest("You have to enter a username");
            if(!user.Email.Contains('@')) return BadRequest("You have to enter a valid email address");
            if(user.Pwd == "" || user.Pwd is null) return BadRequest("You have to enter a password");

            var exist = await UserRepo.ReadAsync(user.Username);

            if(exist is not null) return BadRequest("The username is already taken");

            await UserRepo.CreateAsync(new UserCreateDTO {Username = user.Username, Email = user.Email, Password1 = user.Pwd, Password2 = user.Pwd});
            return NoContent();
        }

        [HttpGet("/sim/{username}")]
        public async Task<IActionResult> get_user(string username)
        {
            await write_latest();

            var user = await UserRepo.ReadAsync(username);
            if (user is null) return NotFound();

            return new ContentResult{
                ContentType = "text/json",
                StatusCode = Status200OK,
                Content = JsonSerializer.Serialize(new {
                    username = user.username,
                    email = user.email
                })
            };
        }

        [HttpGet("/msgs")]
        public async Task<IActionResult> messages()
        {
            await write_latest();

            var noOfMessages = get_param_int("no", 100);
            var messages = await MessageRepo.ReadAllAsync(noOfMessages);
            var selectedMessages = messages.Select(m => new {content = m.text, user = m.author.username});

            return new ContentResult {
                ContentType = "text/json",
                StatusCode = Status200OK,
                Content = JsonSerializer.Serialize(selectedMessages)
            };
        }

        [HttpPost("/msgs/{username}")]
        public async Task<IActionResult> user_post_message([FromBody] SimulationMessageCreateDTO message, string username)
        {
            await write_latest();

            var id = await MessageRepo.CreateAsync(message.Content, username);
            if(id == -1) return BadRequest("Message could not be recorded");
            return NoContent();
        }

        [HttpGet("/msgs/{username}")]
        public async Task<IActionResult> messages_per_user(string username)
        {
            await write_latest();

            var noOfMessages = get_param_int("no", MessageLimit);
            var user = await UserRepo.ReadAsync(username, noOfMessages);

            if (user is null) return NotFound("No such user");

            var messages = user.messages;

            return new  ContentResult {
                ContentType = "text/json",
                StatusCode = Status200OK,
                Content = JsonSerializer.Serialize(messages.ToList())
            };
        }

        [HttpGet("/fllws/{username}")]
        public async Task<IActionResult> hfollow(string username)
        {
            await write_latest();

            var noOfFollows = get_param_int("no", MessageLimit);
            var user = await UserRepo.ReadAsync(username, MessageLimit);
            var follow = user.following.Take(noOfFollows);

            return new ContentResult {
                ContentType = "text/json",
                StatusCode = Status200OK,
                Content = JsonSerializer.Serialize(new {follows = follow})
            };
        }

        [HttpPost("/fllws/{username}")]
        public async Task<IActionResult> follow(string username)
        {
            await write_latest();

            var sr = new StreamReader( Request.Body );
            var bodystring = await sr.ReadToEndAsync();
            string pattern = @"{""(.+?)"": ""(.+?)""}";
            var match = Regex.Matches(bodystring, pattern)[0];

            var method = match.Groups[1].Value;
            var parameter = match.Groups[2].Value;

            int res = 0;

            switch(method)
            {
                case "follow":
                    res = await UserRepo.FollowAsync(username, parameter);
                    break;
                case "unfollow":
                    res = await UserRepo.UnfollowAsync(username, parameter);
                    break;
                default:
                    return BadRequest("Not a supported method");
            }

            if (res != 0) return NotFound();

            return NoContent();
        }
    }
}
