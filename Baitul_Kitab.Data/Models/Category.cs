

using System.ComponentModel.DataAnnotations;

namespace Baitul_Kitab.Models
{
    public class Category : BaseModel
    {
        [Key] // Specifies the primary key
        public Guid Id { get; set; }

        [Required] // Marks the property as required
        [StringLength(100)] // Limits the length of the string
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}
