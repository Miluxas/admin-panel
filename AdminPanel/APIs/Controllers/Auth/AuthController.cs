using System;
using AdminPanel.APIs.Services;
using AdminPanel.APIs.Shared;
using Microsoft.AspNetCore.Mvc;

namespace AdminPanel.APIs.Controllers.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly AuthService service;
        public AuthController(AuthService service)
        {
            this.service = service;
        }

        [HttpPost]
        [Route("register")]
        public async Task<bool> Register(DTOs.RegisterRequestBodyDto newUser)
        {
            return await service.RegisterNewUserAsync(newUser.UserName, newUser.Email, newUser.Password);
        }

        [HttpPost]
        [Route("login")]
        public async Task<LogedInUserInfo> Login(DTOs.LoginRequestBodyDto newUser)
        {
            return await service.LoginAsync(newUser.Email, newUser.Password);
        }
    }
}

