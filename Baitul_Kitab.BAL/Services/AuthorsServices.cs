using Baitul_Kitab.BAL.Interfaces;
using Baitul_Kitab.Data;
using Baitul_Kitab.Data.Models;
using Baitul_Kitab.Models.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Baitul_Kitab.BAL.Services
{
    public class AuthorsServices : IAuthors
    {
        private readonly ApplicationDbContext _context;
        public AuthorsServices(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DTOAuthor>> GetAllAsync()
        {
            return await _context.Authors.Select(a => new DTOAuthor
            {
                Id = a.Id,
                Name = a.Name,
                Biography = a.Biography,
                DateOfBirth = a.DateOfBirth,
                Nationality = a.Nationality,
                IsActive = a.IsActive
            }).ToListAsync();
        }

        public async Task<DTOAuthor?> GetByIdAsync(Guid id)
        {
            var a = await _context.Authors.FindAsync(id);
            if (a == null) return null;
            return new DTOAuthor
            {
                Id = a.Id,
                Name = a.Name,
                Biography = a.Biography,
                DateOfBirth = a.DateOfBirth,
                Nationality = a.Nationality,
                IsActive = a.IsActive
            };
        }

        public async Task<bool> AddAsync(DTOAuthor author)
        {
            var entity = new Author
            {
                Id = Guid.NewGuid(),
                Name = author.Name,
                Biography = author.Biography,
                DateOfBirth = author.DateOfBirth,
                Nationality = author.Nationality,
                IsActive = author.IsActive,
                CreatedBy = author.CreatedBy,
                CreatedOn = DateTime.UtcNow
            };
            _context.Authors.Add(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAsync(DTOAuthor author)
        {
            var entity = await _context.Authors.FindAsync(author.Id);
            if (entity == null) return false;
            entity.Name = author.Name;
            entity.Biography = author.Biography;
            entity.DateOfBirth = author.DateOfBirth;
            entity.Nationality = author.Nationality;
            entity.IsActive = author.IsActive;
            entity.ModifiedBy = author.ModifiedBy;
            entity.ModifiedOn = DateTime.UtcNow;
            _context.Authors.Update(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _context.Authors.FindAsync(id);
            if (entity == null) return false;
            _context.Authors.Remove(entity);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
