using Baitul_Kitab.BAL.Interfaces;
using Baitul_Kitab.Models.DTO.UserStore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Baitul_Kitab.Areas.User.Controllers
{
    [Area("User")]
    [AllowAnonymous]
    public class StoreController : Controller
    {
        private const int HomeBookCount = 8;
        private readonly IUserBooks _userBooks;
        private readonly ICartService _cartService;

        public StoreController(IUserBooks userBooks, ICartService cartService)
        {
            _userBooks = userBooks;
            _cartService = cartService;
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
        public async Task<IActionResult> Categories()
        {
            var categories = await _userBooks.GetCategoriesAsync();
            return View(categories);
        }

        [HttpGet]
        public async Task<IActionResult> Authors()
        {
            var authors = await _userBooks.GetAuthorsAsync();
            return View(authors);
        }

        [HttpGet]
        public async Task<IActionResult> Languages()
        {
            var languages = await _userBooks.GetLanguagesAsync();
            return View(languages);
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

        #region Shopping Cart Endpoints (Public & Authenticated)

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Cart()
        {
            var cart = _cartService.GetCart();
            return View(cart);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(Guid bookId, int quantity = 1, string? returnUrl = null)
        {
            var result = await _cartService.AddItemAsync(bookId, quantity);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new
                {
                    success = result.Success,
                    message = result.Message,
                    count = result.Cart.TotalItems,
                    cartSubtotal = result.Cart.Subtotal.ToString("N2"),
                    bookTitle = result.Item?.Title
                });
            }

            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction(nameof(Cart));
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateCartQuantity(Guid bookId, int quantity)
        {
            var cart = _cartService.UpdateQuantity(bookId, quantity);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                var item = cart.Items.FirstOrDefault(i => i.BookId == bookId);
                return Json(new
                {
                    success = true,
                    count = cart.TotalItems,
                    itemTotal = item != null ? item.Total.ToString("N2") : "0.00",
                    subtotal = cart.Subtotal.ToString("N2"),
                    grandTotal = cart.GrandTotal.ToString("N2"),
                    isEmpty = cart.IsEmpty
                });
            }

            return RedirectToAction(nameof(Cart));
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveFromCart(Guid bookId)
        {
            var cart = _cartService.RemoveItem(bookId);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new
                {
                    success = true,
                    count = cart.TotalItems,
                    subtotal = cart.Subtotal.ToString("N2"),
                    grandTotal = cart.GrandTotal.ToString("N2"),
                    isEmpty = cart.IsEmpty
                });
            }

            TempData["SuccessMessage"] = "Book removed from your shopping cart.";
            return RedirectToAction(nameof(Cart));
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ClearCart()
        {
            _cartService.ClearCart();
            TempData["SuccessMessage"] = "Your shopping cart has been cleared.";
            return RedirectToAction(nameof(Cart));
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetCartCount()
        {
            return Json(new { count = _cartService.GetItemCount() });
        }

        #endregion

        #region Protected Customer Routes

        [Authorize(Roles = "User,Admin")]
        [HttpGet]
        public IActionResult Checkout()
        {
            var cart = _cartService.GetCart();
            if (cart.IsEmpty)
            {
                TempData["ErrorMessage"] = "Your cart is empty. Please add books to cart before proceeding to checkout.";
                return RedirectToAction(nameof(Cart));
            }
            return View();
        }

        [Authorize(Roles = "User,Admin")]
        [HttpGet]
        public IActionResult Orders()
        {
            return View();
        }

        #endregion
    }
}
