using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdminPanel.APIs.Controllers.ToDo.DTOs;
using AdminPanel.APIs.Helper;
using AdminPanel.APIs.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AdminPanel.APIs.Controllers.ToDo
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToDoController : Controller
    {
        private readonly ToDoService service;
        public ToDoController(ToDoService service)
        {
            this.service = service;
        }
        // GET: /<controller>/

        [HttpPost]
        [ApiAuthorization]
        public async Task<bool> Create(CreateRequestBodyDto bodyDto)
        {
            string userId = (string)ControllerContext.HttpContext.Items["UserId"];
            await service.CreateToDo(userId, bodyDto.Title, bodyDto.Effort);

            return true;
        }
    }
}

