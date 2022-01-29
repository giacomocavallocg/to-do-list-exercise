using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoDomain.Sql.Models;

namespace ToDoDomain.Sql.Context
{
    public class ToDoListRepository
    {

        private readonly ToDoDbContext toDoContext;

        public ToDoListRepository(ToDoDbContext dbContext)
        {
            this.toDoContext = dbContext;
        }

        public static IQueryable<ToDoList> FindAll(ToDoDbContext context)
        {
            return context.ToDoLists
                .Include(l => l.Entries)
                .Where(t => t.DeleteDate == null);
        }

        public IQueryable<ToDoList> FindAll()
        {
            return FindAll(toDoContext);
        }

        public async Task<ToDoList> Save(ToDoList toDoList)
        {
            var exists = await FindAll().AnyAsync(t => t.Id == toDoList.Id);

            if (!exists)
            {
                toDoList.CreateDate = DateTime.UtcNow;
                toDoContext.Add(toDoList);
            }
            else
            {
                toDoList.UpdateDate = DateTime.UtcNow;
            }

            return toDoList;
        }

        public Task Delete(ToDoList toDoList)
        {
            var now = DateTime.UtcNow;
            toDoList.DeleteDate = now;
            return new ToDoEntryRepository(toDoContext).DeleteByListId(toDoList.Id, now);

        }

    }
}
