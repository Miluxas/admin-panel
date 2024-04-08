using AdminPanel.Data;
using AdminPanel.Hubs;
using Microsoft.EntityFrameworkCore;

namespace AdminPanel.APIs.Services
{
    public partial class ToDoService
    {
        ApplicationDbContext Context
        {
            get
            {
                return this.context;
            }
        }

        private readonly ApplicationDbContext context;
        private ToDoHub toDoHub;

        public ToDoService(ApplicationDbContext context,ToDoHub toDoHub)
        {

            this.context = context;
            this.toDoHub = toDoHub;
        }

        public async Task<IQueryable<ToDo>> GetToDos()
        {
            var items = Context.ToDos.AsQueryable();
            return await Task.FromResult(items);
        }

        public async Task<ToDo> GetToDoById(Guid id)
        {
            var items = Context.ToDos
                              .AsNoTracking()
                              .Where(i => i.Id == id);

            var itemToReturn = items.FirstOrDefault();

            return await Task.FromResult(itemToReturn);
        }

        public async Task<ToDo> CreateToDo(string userId, string title, double effort)
        {
            ToDo todo = new() { Effort = effort, Title = title, Status = "To Do", UserId = Guid.Parse(userId) };
            try
            {
                Context.ToDos.Add(todo);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(todo).State = EntityState.Detached;
                throw;
            }
            await toDoHub.AddToDo(userId,todo);

            return todo;
        }


        public async Task<ToDo> UpdateToDo(Guid id, ToDo job)
        {

            var itemToUpdate = Context.ToDos
                              .Where(i => i.Id == job.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            Context.Attach(job).State = EntityState.Modified;

            Context.SaveChanges();


            return job;
        }


        public async Task<ToDo> DeleteToDo(Guid id)
        {
            var itemToDelete = Context.ToDos
                              .Where(i => i.Id == id)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }


            Context.ToDos.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }


            return itemToDelete;
        }
    }
}

