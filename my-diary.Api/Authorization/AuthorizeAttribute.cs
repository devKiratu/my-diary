using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace my_diary.Api.Authorization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var IsLoggedIn = context.HttpContext.Items.TryGetValue("User", out _);
            if(!IsLoggedIn)
            {
                context.Result = new UnauthorizedResult();
                    //new JsonResult(new { message = "Unauthorized!" }) {StatusCode = StatusCodes.Status401Unauthorized};
            }
        }
    }
}
