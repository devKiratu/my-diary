using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using my_diary.Api.Model;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BC = BCrypt.Net;


namespace my_diary.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly MainDbContext _dbContext;
        private readonly IConfiguration _config;


        public UsersController(MainDbContext dbContext, IConfiguration config)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignupAsync([FromBody] SignupDto userDto)
        {
            //check whether email address is already registered
            var userExists = await _dbContext.Users.AnyAsync(u => u.Email == userDto.Email);
            if(userExists)
            {
                return BadRequest($"User {userDto.Email} is already registered. Proceed to login, or sign up using a different email address");
            }

            //hash password
            var hashedPwd = BC.BCrypt.EnhancedHashPassword(userDto.Password);

            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                Email = userDto.Email,
                UserName = userDto.UserName,
                Password = hashedPwd,
            };

            await _dbContext.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            return Ok("Registration successful, proceed to login");

        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginDto loginDto)
        {
            //verify user exists
            var user = await _dbContext.Users.Where(u => u.Email == loginDto.Email).FirstOrDefaultAsync();
            if(user == null)
            {
                return BadRequest($"User {loginDto.Email} is not signed up. Please signup first");
            }

            //verify password matches
            var passwordIsValid = BC.BCrypt.EnhancedVerify(loginDto.Password, user.Password);
            if(!passwordIsValid)
            {
                return BadRequest($"Invalid password!");
            }

            //generate token and return it
            var token = GenerateAuthToken(user.Id);
            return Ok(new { msg = "Successfully Logged in!", token });
        }

        private string GenerateAuthToken(string id)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config.GetValue<string>("JwtSecret"));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", id) }),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Expires = DateTime.UtcNow.AddMinutes(60)
                
            };

            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(securityToken);
            return token;
        }
    }
}
