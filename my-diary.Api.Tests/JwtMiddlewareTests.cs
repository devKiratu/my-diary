using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using my_diary.Api.Authorization;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace my_diary.Api.Tests
{
    public class JwtMiddlewareTests
    {

        private readonly IConfiguration _config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
        [Fact]
        public async Task Invoke_NoTokenPresent_NoUserAttachedToContextAsync()
        {
            var httpContext = new DefaultHttpContext();

            static Task next(HttpContext httpContext) => Task.CompletedTask;

            var jwtMiddleware = new JwtMiddleware(next, _config);
            await jwtMiddleware.Invoke(httpContext);

            var IsUserPresent = httpContext.Items.TryGetValue("User", out _);

            Assert.False(IsUserPresent);

        }

        [Fact]
        public async Task Invoke_TokenPresent_AttachesUserToHttpContextAsync()
        {
            //generate token
            var id = "userId01";
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config.GetValue<string>("JwtSecret"));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", id) }),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Expires = DateTime.UtcNow.AddMinutes(1)
            };

            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(securityToken);

            //add token to authorization header
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Authorization"] = $"Bearer {token}";

            static Task next(HttpContext httpContext) => Task.CompletedTask;
            var jwtMiddleware = new JwtMiddleware(next, _config);
            await jwtMiddleware.Invoke(httpContext);

            var IsUserPresent = httpContext.Items.TryGetValue("User", out var idFromContext);

            Assert.True(IsUserPresent);
            Assert.Equal(id, idFromContext.ToString());
        }
    }
}
