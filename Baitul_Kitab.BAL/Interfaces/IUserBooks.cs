using Baitul_Kitab.Models.DTO.UserStore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Baitul_Kitab.BAL.Interfaces
{
    public interface IUserBooks
    {
        /// <summary>Featured/latest visible books for the store home page.</summary>
        Task<IEnumerable<UserBookCardDTO>> GetHomeBooksAsync(int count);

        /// <summary>Searches, filters and pages visible books; also returns filter options.</summary>
        Task<UserBookListDTO> GetBooksAsync(UserBookFilterDTO filter);

        /// <summary>Returns a visible book, or null if missing/deleted/inactive.</summary>
        Task<UserBookDetailDTO?> GetDetailsAsync(Guid id);

        /// <summary>Returns all active categories with book counts for public storefront browsing.</summary>
        Task<List<UserFilterOptionDTO>> GetCategoriesAsync();

        /// <summary>Returns all active authors with book counts for public storefront browsing.</summary>
        Task<List<UserFilterOptionDTO>> GetAuthorsAsync();

        /// <summary>Returns all active languages with book counts for public storefront browsing.</summary>
        Task<List<UserFilterOptionDTO>> GetLanguagesAsync();
    }
}
