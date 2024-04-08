using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AdminPanel.APIs.Helper
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ApiAuthorization : Attribute, IAuthorizationFilter
    {

        public ApiAuthorization()
        {

        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            string userId = (string)context.HttpContext.Items["UserId"];
            if (userId == null)
            {
                context.Result = new JsonResult(
                        new { Message = "Unauthorization" }
                    )
                { StatusCode = StatusCodes.Status401Unauthorized };


            }
        }
    }
}

