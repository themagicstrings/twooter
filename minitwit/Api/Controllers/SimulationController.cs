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

        private async Task<int> get_param_int(string name)
        {
            Microsoft.Extensions.Primitives.StringValues query;
            if(Request.Query.TryGetValue(name, out query))
            {
                int val;
                if(int.TryParse(query.First(), out val)) return val;
                else throw new Exception("param " + name + " is not an int");
            }
            else throw new Exception("No param called: " + name);
        }

        private async Task write_latest()
        {
            try {
                int val = await get_param_int("latest");
                await System.IO.File.WriteAllTextAsync(LATEST, "" + val);
            } catch {
                //ignore
            }
        }

        private async Task<int> read_latest()
        {
            string text = await System.IO.File.ReadAllTextAsync(LATEST);
            return int.Parse(text);
        }

        [HttpGet("/latest")]
        public async Task<IActionResult> get_latest()
        {
            return new ContentResult{
                ContentType = "text/json",
                StatusCode = Status200OK,
                Content = "{\"latest\":"+await read_latest()+"}"
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
            return Ok("You were succesfully registered and can login now");
        }

        [HttpGet("/sim/<username>")]
        public async Task<IActionResult> get_user(string username)
        {
            await write_latest();

            var user = await UserRepo.ReadAsync(username);

            return new ContentResult{
                ContentType = "text/json",
                StatusCode = Status200OK,
                Content = "{\"username\":\""+user.username+"\",\"email\":\""+user.email+"\"}"
            };
        }

        [HttpGet("/msgs")]
        public async Task<IActionResult> messages()
        {
            await write_latest();

            return new ContentResult {
                ContentType = "text/html",
                StatusCode = Status200OK,
                Content = BasicTemplater.GenerateTimeline(await MessageRepo.ReadAllAsync(), user )
            };
        }

        [HttpPost("/msgs/<username>")]
        public async Task<IActionResult> user_post_message([FromBody] MessageCreateDTO message, string username)
        {
            await write_latest();

            var id = await MessageRepo.CreateAsync(message.Text, username);

            if(id == -1) return BadRequest();
            return Ok("Your messages was recorded");
        }

        [HttpGet("/msgs/<username>")]
        public async Task<IActionResult> messages_per_user(string username)
        {
            await write_latest();
            
            var allmessages = await MessageRepo.ReadAllAsync();
            var usermessages = (allmessages.Where(m => m.author.username == username).Select(m => m)).ToList();

            return new  ContentResult {
                ContentType = "text/html",
                StatusCode = Status200OK,
                Content = BasicTemplater.GenerateTimeline(usermessages, user)
            };
        }
    }
}