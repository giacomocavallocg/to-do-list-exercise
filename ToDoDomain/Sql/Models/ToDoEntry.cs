using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ToDoDomain.Sql.Models
{
    public class ToDoEntry
    {

        [Key]
        [Column(TypeName = "varchar(64)")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string ToDoListId { get; set; }
    
        public ToDoList ToDoList { get; set; }

        public string Content { get; set; }

        [Required]
        public bool IsDone { get; set; } = false;

        public DateTime? ExpirationDate { get; set; }

        [Required]
        public DateTime CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }

        public DateTime? DeleteDate { get; set; }

    }
}
