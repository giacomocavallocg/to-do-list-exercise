using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToDoDomain.Sql.Models;

namespace ToDoWebApp.Dtos
{
    public class ToDoListDto
    {

        public string Id { get; set; }

        public string Name { get; set; }

        public bool IsCompleted { get; set; }

        public int NumberOfEntries { get; set; }

        public DateTime CreateDate { get; set; }

        public ToDoListDto() { }
        public ToDoListDto(ToDoList l)
        {
            Id = l.Id;
            Name = l.Name;
            CreateDate = l.CreateDate;
            IsCompleted = l.IsCompleted;
            NumberOfEntries = l.ActiveEntries.Count();
        }

    }
}
