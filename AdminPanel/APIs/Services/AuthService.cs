using System;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using AdminPanel.APIs.Shared;

namespace AdminPanel.APIs.Services
{
    public partial class AuthService
    {
        private readonly IConfiguration configuration;
        private readonly UserManager<IdentityUser> userManager;

        public AuthService(UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.configuration = configuration;
        }


        public async Task<bool> RegisterNewUserAsync(string userName,string email, string password)
        {
            var user = new IdentityUser();
            user.UserName = userName;
            user.Email = email;

            var result = await userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "User");
                return true;
            }
            else
            {
                throw new Exception(result.Errors.First<IdentityError>().Description);
            }
        }
        private async Task<LogedInUserInfo> GenerateToken(IdentityUser user)
        {
            var userRoles = await userManager.GetRolesAsync(user);
            var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                };
            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT: SecretKey"]));
            var token = new JwtSecurityToken(
            issuer: configuration["JWT: ValidIssuer"],
            audience: configuration["JWT: ValidAudience"],
            expires: DateTime.Now.AddMonths(3),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );
            var loginResponse = new LogedInUserInfo() { Id = user.Id, Email = user.Email, UserName = user.UserName, expiration = token.ValidTo, token = new JwtSecurityTokenHandler().WriteToken(token) };
            return loginResponse;
        }

        public async Task<LogedInUserInfo> LoginAsync(string email, string password)
        {
            try
            {
                var user = await userManager.FindByEmailAsync(email);
                if (user != null && await userManager.CheckPasswordAsync(user, password))
                {
                    return await GenerateToken(user);
                }
                throw new UnauthorizedAccessException();
            }
            catch (Exception ex)
            {
                throw new UnauthorizedAccessException();
            }
        }
    }
}

