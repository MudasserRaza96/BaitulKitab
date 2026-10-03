using Baitul_Kitab.Models.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Baitul_Kitab.BAL.Interfaces
{
    public interface IAuthors
    {
        Task<IEnumerable<DTOAuthor>> GetAllAsync();
        Task<DTOAuthor?> GetByIdAsync(System.Guid id);
        Task<bool> AddAsync(DTOAuthor author);
        Task<bool> UpdateAsync(DTOAuthor author);
        Task<bool> DeleteAsync(System.Guid id);
    }
}
