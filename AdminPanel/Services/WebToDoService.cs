using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Radzen;
using AdminPanel.Data;

namespace AdminPanel.Services
{
 partial class WebToDoService
	{
        ApplicationDbContext Context
        {
            get
            {
                return this.context;
            }
        }

        private readonly ApplicationDbContext context;
        private readonly NavigationManager navigationManager;

        public WebToDoService(ApplicationDbContext context, NavigationManager navigationManager)
        {
            this.context = context;
            this.navigationManager = navigationManager;
        }

        public void Reset() => Context.ChangeTracker.Entries().Where(e => e.Entity != null).ToList().ForEach(e => e.State = EntityState.Detached);

        public void ApplyQuery<T>(ref IQueryable<T> items, Query? query = null)
        {
            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Filter))
                {
                    if (query.FilterParameters != null)
                    {
                        items = items.Where(query.Filter, query.FilterParameters);
                    }
                    else
                    {
                        items = items.Where(query.Filter);
                    }
                }

                if (!string.IsNullOrEmpty(query.OrderBy))
                {
                    items = items.OrderBy(query.OrderBy);
                }

                if (query.Skip.HasValue)
                {
                    items = items.Skip(query.Skip.Value);
                }

                if (query.Top.HasValue)
                {
                    items = items.Take(query.Top.Value);
                }
            }
        }



        partial void OnToDosRead(ref IQueryable<ToDo> items);

        public async Task<IQueryable<ToDo>> GetToDos(Query? query = null)
        {
            var items = Context.ToDos.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnToDosRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnJobGet(ToDo item);
        partial void OnGetJobById(ref IQueryable<ToDo> items);


        public async Task<ToDo> GetJobById(Guid id)
        {
            var items = Context.ToDos
                              .AsNoTracking()
                              .Where(i => i.Id == id);


            OnGetJobById(ref items);

            var itemToReturn = items.FirstOrDefault();
            if (itemToReturn != null)
                OnJobGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnJobCreated(ToDo item);
        partial void OnAfterJobCreated(ToDo item);

        public async Task<ToDo> CreateJob(ToDo job)
        {
            OnJobCreated(job);

            var existingItem = Context.ToDos
                              .Where(i => i.Id == job.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.ToDos.Add(job);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(job).State = EntityState.Detached;
                throw;
            }

            OnAfterJobCreated(job);

            return job;
        }

        public async Task<ToDo> CancelJobChanges(ToDo item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnJobUpdated(ToDo item);
        partial void OnAfterJobUpdated(ToDo item);

        public async Task<ToDo> UpdateJob(Guid id, ToDo job)
        {
            OnJobUpdated(job);

            var itemToUpdate = Context.ToDos
                              .Where(i => i.Id == job.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            Reset();

            Context.Attach(job).State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterJobUpdated(job);

            return job;
        }

        partial void OnJobDeleted(ToDo item);
        partial void OnAfterJobDeleted(ToDo item);

        public async Task<ToDo> DeleteJob(Guid id)
        {
            var itemToDelete = Context.ToDos
                              .Where(i => i.Id == id)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnJobDeleted(itemToDelete);

            Reset();

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

            OnAfterJobDeleted(itemToDelete);

            return itemToDelete;
        }
    }
}

