using Baitul_Kitab.Models;
using System.ComponentModel.DataAnnotations;

namespace Baitul_Kitab.Data.Models
{
    public class Author : BaseModel
    {
        [Key]
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Biography { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Nationality { get; set; }
    }
}
