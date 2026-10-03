using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Baitul_Kitab.Models.DTO
{
    public class DataTableParameterDTO
    {
        public int Draw { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
        public DTSearch? Search { get; set; }
        public IEnumerable<DTOrder>? Order { get; set; }
        public IEnumerable<DTColumn>? Columns { get; set; }
    }
    public sealed class DTOrder
    {
        public int Column { get; set; }
        public string? Dir { get; set; }
    }
     public sealed class DTColumn
 {
     /// <summary>
     /// Column's data source
     /// </summary>
     public string? Data { get; set; }

     /// <summary>
     /// Column's name
     /// </summary>
     public string? Name { get; set; }

     /// <summary>
     /// Flag to indicate if this column is orderable (true) or not (false)
     /// </summary>
     public bool? Orderable { get; set; }

     /// <summary>
     /// Flag to indicate if this column is searchable (true) or not (false)
     /// </summary>
     public bool? Searchable { get; set; }

     /// <summary>
     /// Search to apply to this specific column.
     /// </summary>
     public DTSearch? Search { get; set; }
 }

    public sealed class DTSearch
    {
        /// <summary>
        /// Global search value. To be applied to all columns which have searchable as true
        /// </summary>
        public string? Value { get; set; }

        /// <summary>
        /// true if the global filter should be treated as a regular expression for advanced 
        /// searching, false otherwise. Note that normally server-side processing scripts 
        /// will not perform regular expression searching for performance reasons on large 
        /// data sets, but it is technically possible and at the discretion of your script
        /// </summary>
        public bool Regex { get; set; }
    }
}
