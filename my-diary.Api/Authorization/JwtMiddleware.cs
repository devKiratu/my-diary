using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace my_diary.Api.Authorization
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _config;

        public JwtMiddleware(RequestDelegate next, IConfiguration config)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if(token != null)
            {
                AttachUserId(context, token);
            }
            await _next(context);
        }

        private void AttachUserId(HttpContext context, string token)
        {
            //validate token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config.GetValue<string>("JwtSecret"));
            tokenHandler.ValidateToken(token, new TokenValidationParameters 
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.Zero
                
            }, out SecurityToken valiidatedToken);

            //Obtain userId from token and attach it to HttpContext
            var jwtToken = (JwtSecurityToken)valiidatedToken;
            var userId = jwtToken.Claims.First(c => c.Type == "id").Value;

            context.Items["User"] = userId;
        }
    }
}
