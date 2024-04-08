using AdminPanel.Data;
using Microsoft.AspNetCore.SignalR;

namespace AdminPanel.Hubs
{
    public interface IToDoHubClient
    {
        Task AddToDo(ToDo toDo);
    }
    public class ToDoHub : Hub<IToDoHubClient>
    {
        private IHubContext<ToDoHub,IToDoHubClient> hubContext;
        public ToDoHub(IHubContext<ToDoHub, IToDoHubClient> hubContext)
        {
            this.hubContext = hubContext;
        }
        public async Task AddToDo(string userId, ToDo toDo)
        {
            await hubContext.Clients.All.AddToDo( toDo);
        }
    }

}

