using Baitul_Kitab.Models.DTO;
using Baitul_Kitab.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Baitul_Kitab.BAL.Interfaces
{
    public interface ILanguages
    {
        public int AddLanguage(DTOLanguage dTOLanguage);
        public DataTableResponse<DTOLanguage> GetLanguageList(int draw, int start, int length, string sortColumn, string sortDirection, string searchValue);
        public Task<int> UpdateLanguage(DTOLanguage dTOLanguage);
        public Task<bool> DeleteLanguage(Guid? id);
        public Task<IEnumerable<DTOLanguage>> GetAllAsync();

    }
}
