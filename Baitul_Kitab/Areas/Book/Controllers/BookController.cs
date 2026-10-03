using Baitul_Kitab.BAL.Interfaces;
using Baitul_Kitab.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Baitul_Kitab.Areas.Book.Controllers
{
    [Area("Book")]
    [Authorize(Roles = "Admin")]
    public class BookController : Controller
    {
        private readonly IBooks _booksService;
        private readonly ICategories _categoriesService;
        private readonly IAuthors _authorsService;
        private readonly ILanguages _languagesService;
        private readonly IWebHostEnvironment _env;

        public BookController(IBooks booksService, ICategories categoriesService,
            IAuthors authorsService, ILanguages languagesService, IWebHostEnvironment env)
        {
            _booksService = booksService;
            _categoriesService = categoriesService;
            _authorsService = authorsService;
            _languagesService = languagesService;
            _env = env;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> GetCategories()
        {
            var data = await _categoriesService.GetAllAsync();
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetAuthors()
        {
            var data = await _authorsService.GetAllAsync();
            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetLanguages()
        {
            var data = await _languagesService.GetAllAsync();
            return Json(data);
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var books = await _booksService.GetAllAsync();
            return Json(books);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromForm] DTOBook model, IFormFile? BookCoverImageFile, IFormFile? BookPdfFile)
        {
            if (string.IsNullOrWhiteSpace(model.Title))
                return Json(new { result = 0, errorMessage = "Title is required" });

            string? imagePath = null;
            if (BookCoverImageFile != null && BookCoverImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "books");
                Directory.CreateDirectory(uploadsFolder);
                var fileName = Guid.NewGuid() + Path.GetExtension(BookCoverImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                    await BookCoverImageFile.CopyToAsync(stream);
                imagePath = "/uploads/books/" + fileName;
            }

            string? pdfPath = null;
            if (BookPdfFile != null && BookPdfFile.Length > 0)
            {
                var ext = Path.GetExtension(BookPdfFile.FileName).ToLowerInvariant();
                if (ext != ".pdf")
                    return Json(new { result = 0, errorMessage = "Only .pdf files are allowed for Book PDF" });

                var pdfFolder = Path.Combine(_env.WebRootPath, "uploads", "books", "pdf");
                Directory.CreateDirectory(pdfFolder);
                var fileName = Guid.NewGuid() + ".pdf";
                var filePath = Path.Combine(pdfFolder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                    await BookPdfFile.CopyToAsync(stream);
                pdfPath = "/uploads/books/pdf/" + fileName;
            }

            var success = await _booksService.AddAsync(model, imagePath, pdfPath);
            return Json(new { result = success ? 1 : 0, errorMessage = success ? "Book added successfully" : "Failed to add book" });
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromForm] DTOBook model, IFormFile? BookCoverImageFile, IFormFile? BookPdfFile)
        {
            if (model.Id == null || string.IsNullOrWhiteSpace(model.Title))
                return Json(new { result = 0, errorMessage = "Invalid data" });

            string? imagePath = null;
            if (BookCoverImageFile != null && BookCoverImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "books");
                Directory.CreateDirectory(uploadsFolder);
                var fileName = Guid.NewGuid() + Path.GetExtension(BookCoverImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                    await BookCoverImageFile.CopyToAsync(stream);
                imagePath = "/uploads/books/" + fileName;
            }

            string? pdfPath = null;
            if (BookPdfFile != null && BookPdfFile.Length > 0)
            {
                var ext = Path.GetExtension(BookPdfFile.FileName).ToLowerInvariant();
                if (ext != ".pdf")
                    return Json(new { result = 0, errorMessage = "Only .pdf files are allowed for Book PDF" });

                var pdfFolder = Path.Combine(_env.WebRootPath, "uploads", "books", "pdf");
                Directory.CreateDirectory(pdfFolder);
                var fileName = Guid.NewGuid() + ".pdf";
                var filePath = Path.Combine(pdfFolder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                    await BookPdfFile.CopyToAsync(stream);
                pdfPath = "/uploads/books/pdf/" + fileName;
            }

            var success = await _booksService.UpdateAsync(model, imagePath, pdfPath);
            return Json(new { result = success ? 1 : 0, errorMessage = success ? "Book updated successfully" : "Failed to update book" });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _booksService.DeleteAsync(id);
            return Json(new { result = success, errorMessage = success ? "Book deleted successfully" : "Failed to delete book" });
        }
    }
}
