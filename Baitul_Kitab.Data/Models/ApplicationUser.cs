using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Baitul_Kitab.Models
{
    public class ApplicationUser : IdentityUser
    {

        
        
        [StringLength(20)]
        public string? Role { get; set; } = "User";
        public DateTime? CreatedOn { get; set; } = DateTime.Now;
        public Guid? CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; } = DateTime.Now;
        public Guid? ModifiedBy { get; set; }
        public decimal? IsActive { get; set; } = 1;
        public decimal? IsDeleted { get; set; } = null;
        [StringLength(255)]
        public string? FirstName { get; set; }
        [StringLength(255)]
        public string? LastName { get; set; }
       
    }
}
