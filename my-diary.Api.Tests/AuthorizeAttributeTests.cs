using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using my_diary.Api.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace my_diary.Api.Tests
{
    public class AuthorizeAttributeTests
    {
        [Fact]
        public void UserNotLoggedIn_AttachesUnauthorizedResponseToHttpContext()
        {
            //default context with no user credentials
            var actionContext = new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor());
            var context = new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());
            var authAttribute = new AuthorizeAttribute();
            
            authAttribute.OnAuthorization(context);

            Assert.NotNull(context.Result);
            Assert.Equal(typeof(UnauthorizedResult), context.Result.GetType());
        }
        [Fact]
        public void UserLoggedIn_ContextResultIsEmpty()
        {
            //default context with user credentials
            var httpContext = new DefaultHttpContext();
            httpContext.Items["User"] = "userId01";
            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
            var context = new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());
            var authAttribute = new AuthorizeAttribute();

            authAttribute.OnAuthorization(context);

            Assert.Null(context.Result);
        }
    }
}
