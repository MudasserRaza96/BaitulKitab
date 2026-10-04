using Baitul_Kitab.BAL.Interfaces;
using Baitul_Kitab.Data;
using Baitul_Kitab.Models;
using Baitul_Kitab.Models.DTO.UserStore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Baitul_Kitab.BAL.Services
{
    public class UserBooksServices : IUserBooks
    {
        public const int PageSize = 12;

        private readonly ApplicationDbContext _context;

        public UserBooksServices(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>Only active, non-deleted books are ever visible to users.</summary>
        private IQueryable<Book> VisibleBooks() =>
            _context.Books.AsNoTracking().Where(b => b.IsDeleted != true && b.IsActive);

        private static readonly Expression<Func<Book, UserBookCardDTO>> ToCard = b => new UserBookCardDTO
        {
            Id = b.Id,
            Title = b.Title,
            AuthorName = b.Author != null ? b.Author.Name : null,
            CategoryName = b.Category != null ? b.Category.Name : null,
            LanguageName = b.Language != null ? b.Language.Name : null,
            BookCoverImage = b.BookCoverImage,
            PdfFilePath = b.PdfFilePath,
            Price = b.Price,
            DiscountPrice = b.DiscountPrice,
            Quantity = b.Quantity
        };

        public async Task<IEnumerable<UserBookCardDTO>> GetHomeBooksAsync(int count)
        {
            return await VisibleBooks()
                .OrderByDescending(b => b.IsFeatured)
                .ThenByDescending(b => b.CreatedOn)
                .Take(count)
                .Select(ToCard)
                .ToListAsync();
        }

        public async Task<UserBookListDTO> GetBooksAsync(UserBookFilterDTO filter)
        {
            filter ??= new UserBookFilterDTO();
            filter.Search = string.IsNullOrWhiteSpace(filter.Search) ? null : filter.Search.Trim();
            if (filter.MinPrice < 0) filter.MinPrice = null;
            if (filter.MaxPrice < 0) filter.MaxPrice = null;
            if (filter.MinPrice > filter.MaxPrice)
                (filter.MinPrice, filter.MaxPrice) = (filter.MaxPrice, filter.MinPrice);

            var query = VisibleBooks();

            if (filter.Search != null)
                query = query.Where(b => b.Title != null && b.Title.Contains(filter.Search));
            if (filter.CategoryId.HasValue)
                query = query.Where(b => b.CategoryId == filter.CategoryId);
            if (filter.AuthorId.HasValue)
                query = query.Where(b => b.AuthorId == filter.AuthorId);
            if (filter.LanguageId.HasValue)
                query = query.Where(b => b.LanguageId == filter.LanguageId);

            // Price filters apply to the price the customer actually pays (discount when valid).
            if (filter.MinPrice.HasValue)
            {
                var min = filter.MinPrice.Value;
                query = query.Where(b => (b.DiscountPrice != null && b.DiscountPrice > 0 && b.DiscountPrice < b.Price
                    ? b.DiscountPrice.Value : b.Price) >= min);
            }
            if (filter.MaxPrice.HasValue)
            {
                var max = filter.MaxPrice.Value;
                query = query.Where(b => (b.DiscountPrice != null && b.DiscountPrice > 0 && b.DiscountPrice < b.Price
                    ? b.DiscountPrice.Value : b.Price) <= max);
            }

            var total = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(total / (double)PageSize);
            var page = Math.Max(1, filter.Page);
            if (totalPages > 0 && page > totalPages) page = totalPages;
            filter.Page = page;

            var books = await query
                .OrderByDescending(b => b.CreatedOn)
                .ThenBy(b => b.Title)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .Select(ToCard)
                .ToListAsync();

            return new UserBookListDTO
            {
                Filter = filter,
                Books = books,
                TotalCount = total,
                PageSize = PageSize,
                Page = page,
                Categories = await _context.Categories.AsNoTracking()
                    .Where(c => c.IsDeleted != true && c.IsActive).OrderBy(c => c.Name)
                    .Select(c => new UserFilterOptionDTO { Id = c.Id, Name = c.Name }).ToListAsync(),
                Authors = await _context.Authors.AsNoTracking()
                    .Where(a => a.IsDeleted != true && a.IsActive).OrderBy(a => a.Name)
                    .Select(a => new UserFilterOptionDTO { Id = a.Id, Name = a.Name }).ToListAsync(),
                Languages = await _context.Languages.AsNoTracking()
                    .Where(l => l.IsDeleted != true && l.IsActive).OrderBy(l => l.Name)
                    .Select(l => new UserFilterOptionDTO { Id = l.Id, Name = l.Name }).ToListAsync()
            };
        }

        public async Task<UserBookDetailDTO?> GetDetailsAsync(Guid id)
        {
            return await VisibleBooks()
                .Where(b => b.Id == id)
                .Select(b => new UserBookDetailDTO
                {
                    Id = b.Id,
                    Title = b.Title,
                    Description = b.Description,
                    AuthorName = b.Author != null ? b.Author.Name : null,
                    CategoryName = b.Category != null ? b.Category.Name : null,
                    LanguageName = b.Language != null ? b.Language.Name : null,
                    BookCoverImage = b.BookCoverImage,
                    ISBN = b.ISBN,
                    Publisher = b.Publisher,
                    PublishDate = b.PublishDate,
                    Pages = b.Pages,
                    Price = b.Price,
                    DiscountPrice = b.DiscountPrice,
                    Quantity = b.Quantity,
                    PdfFilePath = b.PdfFilePath
                })
                .FirstOrDefaultAsync();
        }

        public async Task<List<UserFilterOptionDTO>> GetCategoriesAsync()
        {
            return await _context.Categories.AsNoTracking()
                .Where(c => c.IsDeleted != true && c.IsActive)
                .OrderBy(c => c.Name)
                .Select(c => new UserFilterOptionDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    BookCount = _context.Books.Count(b => b.CategoryId == c.Id && b.IsDeleted != true && b.IsActive)
                })
                .ToListAsync();
        }

        public async Task<List<UserFilterOptionDTO>> GetAuthorsAsync()
        {
            return await _context.Authors.AsNoTracking()
                .Where(a => a.IsDeleted != true && a.IsActive)
                .OrderBy(a => a.Name)
                .Select(a => new UserFilterOptionDTO
                {
                    Id = a.Id,
                    Name = a.Name,
                    BookCount = _context.Books.Count(b => b.AuthorId == a.Id && b.IsDeleted != true && b.IsActive)
                })
                .ToListAsync();
        }

        public async Task<List<UserFilterOptionDTO>> GetLanguagesAsync()
        {
            return await _context.Languages.AsNoTracking()
                .Where(l => l.IsDeleted != true && l.IsActive)
                .OrderBy(l => l.Name)
                .Select(l => new UserFilterOptionDTO
                {
                    Id = l.Id,
                    Name = l.Name,
                    BookCount = _context.Books.Count(b => b.LanguageId == l.Id && b.IsDeleted != true && b.IsActive)
                })
                .ToListAsync();
        }
    }
}
