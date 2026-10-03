using Baitul_Kitab.Models.DTO;
using Baitul_Kitab.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Baitul_Kitab.BAL.Interfaces
{
    public interface ICategories
    {
        public int AddCategory(CategoryDTO categoryDTO);
        public Task<int> UpdateCategory(CategoryDTO categoryDTO);
        public Task<bool> DeleteCategory(Guid? id);
        public DataTableResponse<CategoryDTO> GetCategoryList(int draw, int start, int length, string sortColumn, string sortDirection, string searchValue);
        public Task<IEnumerable<CategoryDTO>> GetAllAsync();


    }
}
