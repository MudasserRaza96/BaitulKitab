using Baitul_Kitab.BAL.Interfaces;
using Baitul_Kitab.Data;
using Baitul_Kitab.Data.Models;
using Baitul_Kitab.Models.DTO;
using Baitul_Kitab.Models.ViewModels;
using Baitul_Kitab.Models.Helper;
using Microsoft.EntityFrameworkCore;

namespace Baitul_Kitab.BAL.Services
{
    public class LanguagesServices: ILanguages
    {
        private readonly ApplicationDbContext applicationDbContext;

        public LanguagesServices(ApplicationDbContext applicationDbContext)
        {
            this.applicationDbContext = applicationDbContext;
        }

        public int AddLanguage(DTOLanguage dtoLanguage)
        {
            try
            {
                var language = new Language
                {
                    Id = Guid.NewGuid(),
                    Name = dtoLanguage.Name,
                    Description = dtoLanguage.Description,
                    CreatedBy = dtoLanguage.CreatedBy,
                    CreatedOn = DateTime.Now,
                    IsActive = (bool)dtoLanguage.IsActive,
                    IsDeleted = false
                };
                var isExists = applicationDbContext.Languages
                .Any(x => x.Name.ToLower() == language.Name.ToLower());

                if (isExists)
                {
                    return -5;   // Name already exists
                }

                applicationDbContext.Languages.Add(language);
                applicationDbContext.SaveChanges();

                return 2;
            }
            catch (Exception ex)
            {
                // You can log the exception here
                return -1;
            }
        }

        public async Task<bool> DeleteLanguage(Guid? id)
        {
            try
            {
                var category = await applicationDbContext.Languages.FindAsync(id);
                if (category == null)
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
        public async Task<int> UpdateLanguage(DTOLanguage dTOLanguage)
        {
            try
            {
                var category = await applicationDbContext.Languages.FindAsync(dTOLanguage.Id);
                if (category == null)
                {
                    return -1;
                }
                var isExists = applicationDbContext.Languages
                    .Any(x => x.Name.ToLower() == dTOLanguage.Name.ToLower() && x.Id != dTOLanguage.Id && !(bool)x.IsDeleted);

                if (isExists)
                {
                    return -5;   // Name already exists
                }
                category.Name = dTOLanguage.Name;
                category.Description = dTOLanguage.Description;
                category.ModifiedBy = dTOLanguage.CreatedBy;
                category.ModifiedOn = DateTime.Now;
                category.IsActive = (bool)dTOLanguage.IsActive;
                await applicationDbContext.SaveChangesAsync();
                return 2;
            }
            catch
            {
                return -1; // Handle exceptions as needed
            }
        }
        public DataTableResponse<DTOLanguage> GetLanguageList(int draw, int start, int length, string sortColumn, string sortDirection, string searchValue)
        {
            var query = applicationDbContext.Languages
            .Where(x => x.IsDeleted != true);

            // Filtering
            if (!string.IsNullOrEmpty(searchValue))
            {
                query = query.Where(x => x.Name.Contains(searchValue));
            }

            var totalRecords = query.Count();

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
                query = query.OrderBy(x => x.Id);
            }

            // Paging
            var data = query
                .Skip(start)
                .Take(length)
                .Select(x => new DTOLanguage
                {
                    Id = x.Id,
                    Name = x.Name,
                    CreatedOn = x.CreatedOn,
                    IsActive = x.IsActive,
                    Description = x.Description,
                })
                .ToList();

            return new DataTableResponse<DTOLanguage>
            {
                draw = draw,
                recordsTotal = totalRecords,
                recordsFiltered = totalRecords,
                data = data
            };

        }

        public async Task<IEnumerable<DTOLanguage>> GetAllAsync()
        {
            return await applicationDbContext.Languages
                .Where(x => x.IsDeleted != true && x.IsActive == true)
                .Select(x => new DTOLanguage
                {
                    Id = x.Id,
                    Name = x.Name,
                })
                .ToListAsync();
        }

    }
}
