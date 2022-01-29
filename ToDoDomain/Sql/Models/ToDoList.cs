using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace ToDoDomain.Sql.Models
{
    public class ToDoList
    {

        [Key]
        [Column(TypeName = "varchar(64)")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string Name { get; set; }

        [Required]
        public DateTime CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }

        public DateTime? DeleteDate { get; set; }


        public List<ToDoEntry> Entries { get; set; } = new List<ToDoEntry>();


        // NOT MAPPED

        [NotMapped]
        public List<ToDoEntry> ActiveEntries => Entries.Where(e => e.DeleteDate == null).ToList();

        [NotMapped]
        public bool IsCompleted => !ActiveEntries.Any(e => e.IsDone == false);

    }
}
