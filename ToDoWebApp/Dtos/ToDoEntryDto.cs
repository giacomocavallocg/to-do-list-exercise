using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ToDoDomain.Sql.Models;

namespace ToDoWebApp.Dtos
{
    public class ToDoEntryDto
    {

        public string Id { get; set; }

        [Required]
        public string ToDoListId { get; set; }

        public string Content { get; set; }

        public bool IsDone { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public DateTime CreateDate { get; set; }

        public bool IsExpired => DateTime.UtcNow > ExpirationDate;

        public ToDoEntryDto() { }

        public ToDoEntryDto(ToDoEntry e)
        {
            Id = e.Id;
            ToDoListId = e.ToDoListId;
            CreateDate = e.CreateDate;
            IsDone = e.IsDone;
            ExpirationDate = e.ExpirationDate;
            Content = e.Content;
        }

    }
}
