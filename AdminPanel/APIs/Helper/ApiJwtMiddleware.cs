using System;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace AdminPanel.APIs.Helper
{
    public class ApiJwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration configuration;

        public ApiJwtMiddleware(RequestDelegate _next, IConfiguration configuration)
        {
            this._next = _next;
            this.configuration = configuration;

        }
        public Task Invoke(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (token != null)
                //Validate Token
                attachUserToContext(context, token);
            return _next(context);
        }

        private void attachUserToContext(HttpContext context, string token)
        {

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT: SecretKey"]));
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = configuration["JWT: ValidIssuer"],
                    ValidAudience = configuration["JWT: ValidAudience"]
                }, out SecurityToken validateToken);

                var jwtToken = (JwtSecurityToken)validateToken;
                var userId = jwtToken.Claims.FirstOrDefault(_ => _.Type == ClaimTypes.NameIdentifier);
                context.Items["UserId"] = userId != null ? userId.Value.ToString() : null;

            }
            catch (Exception ex)
            {


            }
        }
    }
}

