using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoDomain.Sql.Models;

namespace ToDoDomain.Sql.Context
{
    public class ToDoEntryRepository
    {

        private readonly ToDoDbContext toDoContext;

        public ToDoEntryRepository(ToDoDbContext dbContext)
        {
            this.toDoContext = dbContext;
        }

        public static IQueryable<ToDoEntry> FindAll(ToDoDbContext context)
        {
            return context.ToDoEntries.Where(t => t.DeleteDate == null);
        }

        public IQueryable<ToDoEntry> FindAll()
        {
            return FindAll(toDoContext);
        }

        public async Task<ToDoEntry> Save(ToDoEntry toDoEntry)
        {
            var exists = await FindAll().AnyAsync(t => t.Id == toDoEntry.Id);

            if (!exists)
            {
                toDoEntry.CreateDate = DateTime.UtcNow;
                toDoContext.Add(toDoEntry);
            }
            else
            {
                toDoEntry.UpdateDate = DateTime.UtcNow;
            }

            return toDoEntry;
        }

        public void Delete(ToDoEntry toDoEntry)
        {
            toDoEntry.DeleteDate = DateTime.UtcNow;
        }

        internal Task DeleteByListId(string toDoListId, DateTime deletionDate)
        {
            var updateQuery = @"
                UPDATE MD_ToDoEntries
                    SET DeleteDate = {0}
                    WHERE ToDoListId = {1} and DeleteDate is null;
            ";

            return toDoContext.Database.ExecuteSqlRawAsync(updateQuery, deletionDate, toDoListId);
        }

    }
}
