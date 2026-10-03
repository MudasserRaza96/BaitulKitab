using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Baitul_Kitab.Models
{
    public class Order : BaseModel
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }  // Foreign key for User
        public ApplicationUser  UserName { get; set; }    // Navigation property

        public int BookId { get; set; }  // Foreign key for Book
        public Book BookName { get; set; }   // Navigation property

        [Range(1, 100)] // Validates the range for Quantity
        public int Quantity { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now; 
    }
}
