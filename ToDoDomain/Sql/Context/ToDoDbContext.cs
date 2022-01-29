using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using ToDoDomain.Sql.Models;

namespace ToDoDomain.Sql.Context
{
    public class ToDoDbContext: DbContext
    {


        public ToDoDbContext()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var builder = new DbContextOptionsBuilder<ToDoDbContext>()
                .UseSqlite(connection)
                .Options;

        }

        public ToDoDbContext(DbContextOptions<ToDoDbContext> options) : base(options)
        {
            var exists = this.Database.EnsureCreated();

            if (!exists)
            {
                var x = 1;
            }

            var u = 2;
        }

        public DbSet<ToDoList> ToDoLists { get; set; }

        public DbSet<ToDoEntry> ToDoEntries { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ToDoList>().ToTable("MD_ToDoLists");
            builder.Entity<ToDoEntry>().ToTable("MD_ToDoEntries");

        }

    }
}
