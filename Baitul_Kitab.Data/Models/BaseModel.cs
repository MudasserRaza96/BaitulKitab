using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Baitul_Kitab.Models
{
    public class BaseModel
    {
        public BaseModel()
        {
            CreatedOn = DateTime.Now;
        }
        public Guid? CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }

        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; } 

        public bool? IsDeleted { get; set; }
        public bool IsActive { get; set; } = true;
    }
}

