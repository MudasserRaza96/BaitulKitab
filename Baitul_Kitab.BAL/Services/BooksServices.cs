using Baitul_Kitab.BAL.Interfaces;
using Baitul_Kitab.Data;
using Baitul_Kitab.Models;
using Baitul_Kitab.Models.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Baitul_Kitab.BAL.Services
{
    public class BooksServices : IBooks
    {
        private readonly ApplicationDbContext _context;
        public BooksServices(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DTOBook>> GetAllAsync()
        {
            return await _context.Books
                .Where(b => b.IsDeleted != true)
                .Include(b => b.Category)
                .Include(b => b.Author)
                .Include(b => b.Language)
                .Select(b => new DTOBook
                {
                    Id = b.Id,
                    Title = b.Title,
                    Description = b.Description,
                    CategoryId = b.CategoryId,
                    CategoryName = b.Category != null ? b.Category.Name : null,
                    AuthorId = b.AuthorId,
                    AuthorName = b.Author != null ? b.Author.Name : null,
                    LanguageId = b.LanguageId,
                    LanguageName = b.Language != null ? b.Language.Name : null,
                    ISBN = b.ISBN,
                    Price = b.Price,
                    DiscountPrice = b.DiscountPrice,
                    Quantity = b.Quantity,
                    Publisher = b.Publisher,
                    PublishDate = b.PublishDate,
                    Pages = b.Pages,
                    BookCoverImage = b.BookCoverImage,
                    PdfFilePath = b.PdfFilePath,
                    IsFeatured = b.IsFeatured,
                    IsActive = b.IsActive
                }).ToListAsync();
        }

        public async Task<DTOBook?> GetByIdAsync(Guid id)
        {
            var b = await _context.Books
                .Include(x => x.Category)
                .Include(x => x.Author)
                .Include(x => x.Language)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (b == null) return null;
            return new DTOBook
            {
                Id = b.Id,
                Title = b.Title,
                Description = b.Description,
                CategoryId = b.CategoryId,
                CategoryName = b.Category != null ? b.Category.Name : null,
                AuthorId = b.AuthorId,
                AuthorName = b.Author != null ? b.Author.Name : null,
                LanguageId = b.LanguageId,
                LanguageName = b.Language != null ? b.Language.Name : null,
                ISBN = b.ISBN,
                Price = b.Price,
                DiscountPrice = b.DiscountPrice,
                Quantity = b.Quantity,
                Publisher = b.Publisher,
                PublishDate = b.PublishDate,
                Pages = b.Pages,
                BookCoverImage = b.BookCoverImage,
                PdfFilePath = b.PdfFilePath,
                IsFeatured = b.IsFeatured,
                IsActive = b.IsActive
            };
        }

        public async Task<bool> AddAsync(DTOBook book, string? imagePath, string? pdfPath = null)
        {
            var entity = new Book
            {
                Id = Guid.NewGuid(),
                Title = book.Title,
                Description = book.Description,
                CategoryId = book.CategoryId,
                AuthorId = book.AuthorId,
                LanguageId = book.LanguageId,
                ISBN = book.ISBN,
                Price = book.Price,
                DiscountPrice = book.DiscountPrice,
                Quantity = book.Quantity,
                Publisher = book.Publisher,
                PublishDate = book.PublishDate,
                Pages = book.Pages,
                BookCoverImage = imagePath ?? book.BookCoverImage,
                PdfFilePath = pdfPath ?? book.PdfFilePath,
                IsFeatured = book.IsFeatured,
                IsActive = book.IsActive,
                IsDeleted = false,
                CreatedBy = book.CreatedBy,
                CreatedOn = DateTime.UtcNow
            };
            _context.Books.Add(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAsync(DTOBook book, string? imagePath, string? pdfPath = null)
        {
            var entity = await _context.Books.FindAsync(book.Id);
            if (entity == null) return false;
            entity.Title = book.Title;
            entity.Description = book.Description;
            entity.CategoryId = book.CategoryId;
            entity.AuthorId = book.AuthorId;
            entity.LanguageId = book.LanguageId;
            entity.ISBN = book.ISBN;
            entity.Price = book.Price;
            entity.DiscountPrice = book.DiscountPrice;
            entity.Quantity = book.Quantity;
            entity.Publisher = book.Publisher;
            entity.PublishDate = book.PublishDate;
            entity.Pages = book.Pages;
            if (imagePath != null)
                entity.BookCoverImage = imagePath;
            if (pdfPath != null)
                entity.PdfFilePath = pdfPath;
            entity.IsFeatured = book.IsFeatured;
            entity.IsActive = book.IsActive;
            entity.ModifiedBy = book.ModifiedBy;
            entity.ModifiedOn = DateTime.UtcNow;
            _context.Books.Update(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _context.Books.FindAsync(id);
            if (entity == null) return false;
            entity.IsDeleted = true;
            entity.ModifiedOn = DateTime.UtcNow;
            _context.Books.Update(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<DTOBook>> GetDropdownAsync()
        {
            return await _context.Books
                .Where(b => b.IsDeleted != true && b.IsActive)
                .Select(b => new DTOBook { Id = b.Id, Title = b.Title })
                .ToListAsync();
        }
    }
}
