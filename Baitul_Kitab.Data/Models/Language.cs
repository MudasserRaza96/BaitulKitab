
using Baitul_Kitab.Models;
using System.ComponentModel.DataAnnotations;

namespace Baitul_Kitab.Data.Models
{
    public class Language : BaseModel
    {
        [Key]
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }

    }
}
