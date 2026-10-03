using Baitul_Kitab.BAL.Interfaces;
using Baitul_Kitab.Data;
using Baitul_Kitab.Models;
using Baitul_Kitab.Models.DTO;
using Baitul_Kitab.Models.DTO.CommonModel;
using Baitul_Kitab.Models.Helper;
using Baitul_Kitab.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Baitul_Kitab.BAL.Services
{
    public class CategoriesServices : ICategories
    {
        private readonly ApplicationDbContext applicationDbContext;

        public CategoriesServices(ApplicationDbContext applicationDbContext)
        {
            this.applicationDbContext = applicationDbContext;
        }

        public int AddCategory(CategoryDTO categoryDTO)
        {
            try
            {
                var category = new Category
                {
                    Id = Guid.NewGuid(),
                    Name = categoryDTO.Name,
                    Description = categoryDTO.Description,
                    CreatedBy = categoryDTO.CreatedBy,
                    CreatedOn = DateTime.Now,
                    IsActive = true,
                    IsDeleted = false
                };
                var isExists = applicationDbContext.Categories
                .Any(x => x.Name.ToLower() == category.Name.ToLower());
                if (isExists)
                {
                    return -5;   // Name already exists
                }

                applicationDbContext.Categories.Add(category);
                applicationDbContext.SaveChanges();

                return 2;
            }
            catch (Exception ex)
            {
                // You can log the exception here
                return -1;
            }
        }

        public DataTableResponse<CategoryDTO> GetCategoryList(int draw,int start,int length,string sortColumn,string sortDirection,string searchValue)
        {
            // Base query excluding deleted categories
            var query = applicationDbContext.Categories
            .Where(x => x.IsDeleted != true);

            // Filtering
            if (!string.IsNullOrEmpty(searchValue))
            {
                query = query.Where(x => x.Name.Contains(searchValue));
            }

            var totalRecords = query.Count();

            // Dynamic sorting using your extension
            if (!string.IsNullOrEmpty(sortColumn))
            {
                try
                {
                    query = sortDirection == "asc"
                        ? query.OrderBy(sortColumn)
                        : query.OrderByDescending(sortColumn);
                }
                catch (ArgumentException)
                {
                    // Optional: fallback if property name is invalid
                    query = query.OrderBy(x => x.Id);
                }
            }
            else
            {
                // Default ordering
                query = query.OrderBy(x => x.Id);
            }

            // Paging
            var data = query
                .Skip(start)
                .Take(length)
                .Select(x => new CategoryDTO
                {
                    Id = x.Id,
                    Name = x.Name,
                    CreatedOn = x.CreatedOn,
                    IsActive = x.IsActive,
                    Description = x.Description,    
                })
                .ToList();

            return new DataTableResponse<CategoryDTO>
            {
                draw = draw,
                recordsTotal = totalRecords,
                recordsFiltered = totalRecords,
                data = data
            };

}

        public async Task<bool> DeleteCategory(Guid? id)
        {
            try
            {
                var category = await applicationDbContext.Categories.FindAsync(id);
                if(category == null)
                {
                    return false;
                }
                category.IsDeleted = true;
                await applicationDbContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false; // Handle exceptions as needed
            }
        }
        public async Task<IEnumerable<CategoryDTO>> GetAllAsync()
        {
            return await applicationDbContext.Categories
                .Where(x => x.IsDeleted != true && x.IsActive == true)
                .Select(x => new CategoryDTO
                {
                    Id = x.Id,
                    Name = x.Name,
                })
                .ToListAsync();
        }

        public async Task<int> UpdateCategory(CategoryDTO categoryDTO)
        {
            try
            {
                var category = await applicationDbContext.Categories.FindAsync(categoryDTO.Id);
                if(category == null)
                {
                    return -1;
                }
                var isExists = applicationDbContext.Categories
                    .Any(x => x.Name.ToLower() == categoryDTO.Name.ToLower() && x.Id != categoryDTO.Id && !(bool)x.IsDeleted  );

                if (isExists)
                {
                    return -5;   // Name already exists
                }
                category.Name = categoryDTO.Name;
                    category.Description = categoryDTO.Description;
                    category.ModifiedBy = categoryDTO.CreatedBy;
                    category.ModifiedOn = DateTime.Now;
                    category.IsActive = (bool)categoryDTO.IsActive;
                await applicationDbContext.SaveChangesAsync();
                return 2;
            }
            catch
            {
                return -1; // Handle exceptions as needed
            }
        }

    }
}
