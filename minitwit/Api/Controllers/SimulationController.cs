using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Models;
using Shared;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Web;
using System.Linq;
using System;
using System.Text.Json;
using System.IO;
using Microsoft.Extensions.Logging;
using static Api.TwooterOptions;
using static Shared.CreateReturnType;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Api.Controllers
{
    public class SimulationController : Controller
    {
        private readonly IMessageRepository MessageRepo;
        private readonly IUserRepository UserRepo;
        private readonly SessionHelper sessionHelper;
        private ILogger<SimulationController> logger;

        private readonly string LATEST = "./LATEST.txt";


        public SimulationController(IMessageRepository msgrepo, IUserRepository usrrepo, ILogger<SimulationController> logger)
        {
            this.MessageRepo = msgrepo;
            this.UserRepo = usrrepo;
            this.sessionHelper = new SessionHelper(() => HttpContext.Session);
            this.logger = logger;
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

        private bool reqFromSimulator (out UnauthorizedObjectResult result)
        {
            Request.Headers.TryGetValue("Authorization", out var authToken);
            if (authToken == "Basic c2ltdWxhdG9yOnN1cGVyX3NhZmUh") {
                result = null;
                return true;
            }
            result = Unauthorized("You are not authorized to use this resource!");
            return false;
        }

        [HttpGet("/latest")]
        public async Task<IActionResult> get_latest()
        {
            if (!reqFromSimulator(out var result)) return result;
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
            if (!reqFromSimulator(out var result)) return result;
            await write_latest();

            var res = await UserRepo.CreateAsync(new UserCreateDTO {Username = user.Username, Email = user.Email, Password1 = user.Pwd, Password2 = user.Pwd});
            
            switch(res)
            {
                case MISSING_PASSWORD:
                    return BadRequest("You have to enter a password");
                case MISSING_USERNAME:
                    return BadRequest("You have to enter a username");
                case INVALID_EMAIL:
                    return BadRequest("You have to enter a valid email address");
                case PASSWORD_MISMATCH:
                    return BadRequest("Passwords are not matching");
                case USERNAME_TAKEN:
                    return BadRequest("The username is already taken");
                case EMAIL_TAKEN:
                    return BadRequest("The email is already taken");
                case SUCCES:
                default:
                    MinitwitController.TotalUsers.IncTo(UserRepo.GetTotalUsers());
                    return NoContent();
            }
        }

        [HttpGet("/sim/{username}")]
        public async Task<IActionResult> get_user([FromRoute] string username)
        {
            if (!reqFromSimulator(out var result)) return result;
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
        public async Task<IActionResult> messages([FromQuery] int no = 100)
        {
            if (!reqFromSimulator(out var result)) return result;
            await write_latest();

            var messages = await MessageRepo.ReadAllAsync(no);
            var selectedMessages = messages.Select(m => new {content = m.text, user = m.author.username});

            return new ContentResult {
                ContentType = "text/json",
                StatusCode = Status200OK,
                Content = JsonSerializer.Serialize(selectedMessages)
            };
        }

        [HttpPost("/msgs/{username}")]
        public async Task<IActionResult> user_post_message([FromBody] SimulationMessageCreateDTO message, [FromRoute] string username)
        {
            logger.LogInformation($"SIMULATION: {username} is posting a message");
            if (!reqFromSimulator(out var result)) return result;
            await write_latest();

            var id = await MessageRepo.CreateAsync(message.Content, username);
            if(id == -1) 
            {
                logger.LogError($"SIMULATION: {username} does not exist");
                return BadRequest("Message could not be recorded");
            }
            return NoContent();
        }

        [HttpGet("/msgs/{username}")]
        public async Task<IActionResult> messages_per_user([FromRoute] string username, [FromQuery] int no = 100)
        {
            if (!reqFromSimulator(out var result)) return result;
            await write_latest();

            var user = await UserRepo.ReadAsync(username, no);

            if (user is null) return NotFound("No such user");

            var messages = user.messages;
            var selectedMessages = messages.Select(m => new {content = m.text, user = m.author.username});

            return new  ContentResult {
                ContentType = "text/json",
                StatusCode = Status200OK,
                Content = JsonSerializer.Serialize(selectedMessages)
            };
        }

        [HttpGet("/fllws/{username}")]
        public async Task<IActionResult> hfollow([FromRoute] string username, [FromQuery] int no = 100)
        {
            if (!reqFromSimulator(out var result)) return result;
            await write_latest();

            var user = await UserRepo.ReadAsync(username, MessageLimit);
            var follow = user.following.Take(no);

            return new ContentResult {
                ContentType = "text/json",
                StatusCode = Status200OK,
                Content = JsonSerializer.Serialize(new {follows = follow})
            };
        }

        [HttpPost("/fllws/{username}")]
        public async Task<IActionResult> follow([FromRoute] string username)
        {
            if (!reqFromSimulator(out var result)) return result;
            await write_latest();

            var sr = new StreamReader( Request.Body );
            var bodystring = await sr.ReadToEndAsync();
            var body = JsonSerializer.Deserialize<FollowDTO>(bodystring);

            int res = 0;

            if (body.unfollow != null)
            {
                res = await UserRepo.UnfollowAsync(username, body.unfollow);
            }
            else if (body.follow != null)
            {
                res = await UserRepo.FollowAsync(username, body.follow);
            }
            else 
            {
                return BadRequest("Not a supported method");
            }

            if (res != 0) return NotFound();

            return NoContent();
        }
    }
}
