using Baitul_Kitab.Models.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Baitul_Kitab.BAL.Interfaces
{
    public interface IBooks
    {
        Task<IEnumerable<DTOBook>> GetAllAsync();
        Task<DTOBook?> GetByIdAsync(Guid id);
        Task<bool> AddAsync(DTOBook book, string? imagePath, string? pdfPath = null);
        Task<bool> UpdateAsync(DTOBook book, string? imagePath, string? pdfPath = null);
        Task<bool> DeleteAsync(Guid id);
        Task<IEnumerable<DTOBook>> GetDropdownAsync();
    }
}
