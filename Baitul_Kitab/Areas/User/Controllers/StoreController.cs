using Baitul_Kitab.BAL.Interfaces;
using Baitul_Kitab.Models.DTO.UserStore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Baitul_Kitab.Areas.User.Controllers
{
    [Area("User")]
    [Authorize(Roles = "User")]
    public class StoreController : Controller
    {
        private const int HomeBookCount = 8;
        private readonly IUserBooks _userBooks;

        public StoreController(IUserBooks userBooks)
        {
            _userBooks = userBooks;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var books = await _userBooks.GetHomeBooksAsync(HomeBookCount);
            return View(books);
        }

        [HttpGet]
        public async Task<IActionResult> Books(UserBookFilterDTO filter)
        {
            var model = await _userBooks.GetBooksAsync(filter);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            if (id == Guid.Empty)
                return NotFound();

            var book = await _userBooks.GetDetailsAsync(id);
            if (book == null)
                return NotFound();

            return View(book);
        }

        [HttpGet]
        public IActionResult About()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Contact(string name, string email, string? subject, string message)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(message))
            {
                ViewBag.ErrorMessage = "Please fill in all required fields (Name, Email, and Message).";
                return View();
            }

            ViewBag.SuccessMessage = "Thank you for reaching out! Your message has been received and we will get back to you shortly.";
            return View();
        }
    }
}
