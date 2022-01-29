using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToDoDomain.Sql.Context;

namespace ToDoTest.Common
{
    public class ToDoDb : IDisposable
    {
        private SqliteConnection connection;

        public ToDoDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ToDoDbContext>()
                .UseSqlite(connection)
                .Options;


            return new ToDoDbContext(options);
        }

        public ToDoDb()
        {
            connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            using (var context = CreateContext())
            {
                context.Database.EnsureCreated();
                context.SaveChanges();
            }
        }

        public void Dispose()
        {
            connection.Close();
        }
    }
}
