using Baitul_Kitab.BAL.Interfaces;
using Baitul_Kitab.Data;
using Baitul_Kitab.Models.DTO.UserStore;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Baitul_Kitab.BAL.Services
{
    public class CartService : ICartService
    {
        private const string CartCookieKey = "BaitulKitab_Cart";
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _context;

        public CartService(IHttpContextAccessor httpContextAccessor, ApplicationDbContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        private HttpContext? HttpContext => _httpContextAccessor.HttpContext;

        public CartDTO GetCart()
        {
            return ReadCartFromCookie();
        }

        public int GetItemCount()
        {
            var cart = ReadCartFromCookie();
            return cart.TotalItems;
        }

        public async Task<(bool Success, string Message, CartItemDTO? Item, CartDTO Cart)> AddItemAsync(Guid bookId, int quantity = 1)
        {
            if (bookId == Guid.Empty)
                return (false, "Invalid book identifier.", null, GetCart());

            if (quantity <= 0)
                quantity = 1;

            var book = await _context.Books
                .AsNoTracking()
                .Include(b => b.Author)
                .FirstOrDefaultAsync(b => b.Id == bookId && b.IsDeleted != true && b.IsActive);

            if (book == null)
                return (false, "This book is no longer available in the bookstore.", null, GetCart());

            int availableStock = book.Quantity.GetValueOrDefault();
            if (availableStock <= 0)
                return (false, $"\"{book.Title}\" is currently out of stock.", null, GetCart());

            var cart = ReadCartFromCookie();
            var existingItem = cart.Items.FirstOrDefault(i => i.BookId == bookId);

            decimal effectivePrice = (book.DiscountPrice.HasValue && book.DiscountPrice > 0 && book.DiscountPrice < book.Price)
                ? book.DiscountPrice.Value
                : book.Price;

            if (existingItem != null)
            {
                int newQuantity = existingItem.Quantity + quantity;
                if (newQuantity > availableStock)
                {
                    existingItem.Quantity = availableStock;
                    existingItem.StockQuantity = availableStock;
                    existingItem.Price = effectivePrice;
                    existingItem.OriginalPrice = book.Price;
                    SaveCartToCookie(cart);
                    return (true, $"Maximum available stock ({availableStock}) for \"{book.Title}\" added to cart.", existingItem, cart);
                }

                existingItem.Quantity = newQuantity;
                existingItem.StockQuantity = availableStock;
                existingItem.Price = effectivePrice;
                existingItem.OriginalPrice = book.Price;
                SaveCartToCookie(cart);
                return (true, $"Updated quantity for \"{book.Title}\" in your cart.", existingItem, cart);
            }
            else
            {
                int addQty = Math.Min(quantity, availableStock);
                var newItem = new CartItemDTO
                {
                    BookId = book.Id,
                    Title = book.Title ?? "Untitled Book",
                    AuthorName = book.Author?.Name,
                    CoverImage = book.BookCoverImage,
                    Price = effectivePrice,
                    OriginalPrice = book.Price,
                    Quantity = addQty,
                    StockQuantity = availableStock
                };

                cart.Items.Add(newItem);
                SaveCartToCookie(cart);
                return (true, $"\"{book.Title}\" has been added to your cart.", newItem, cart);
            }
        }

        public CartDTO UpdateQuantity(Guid bookId, int quantity)
        {
            var cart = ReadCartFromCookie();
            var item = cart.Items.FirstOrDefault(i => i.BookId == bookId);
            if (item != null)
            {
                if (quantity <= 0)
                {
                    cart.Items.Remove(item);
                }
                else
                {
                    item.Quantity = item.StockQuantity > 0 ? Math.Min(quantity, item.StockQuantity) : quantity;
                }
                SaveCartToCookie(cart);
            }
            return cart;
        }

        public CartDTO RemoveItem(Guid bookId)
        {
            var cart = ReadCartFromCookie();
            var item = cart.Items.FirstOrDefault(i => i.BookId == bookId);
            if (item != null)
            {
                cart.Items.Remove(item);
                SaveCartToCookie(cart);
            }
            return cart;
        }

        public void ClearCart()
        {
            var cart = new CartDTO();
            SaveCartToCookie(cart);
        }

        private CartDTO ReadCartFromCookie()
        {
            var context = HttpContext;
            if (context == null) return new CartDTO();

            if (context.Request.Cookies.TryGetValue(CartCookieKey, out var cookieValue) && !string.IsNullOrWhiteSpace(cookieValue))
            {
                try
                {
                    var items = JsonSerializer.Deserialize<List<CartItemDTO>>(cookieValue);
                    if (items != null)
                    {
                        return new CartDTO { Items = items };
                    }
                }
                catch
                {
                    // Fallback to empty if corrupted
                }
            }

            return new CartDTO();
        }

        private void SaveCartToCookie(CartDTO cart)
        {
            var context = HttpContext;
            if (context == null) return;

            var cookieOptions = new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(30),
                HttpOnly = true,
                IsEssential = true,
                SameSite = SameSiteMode.Lax,
                Secure = context.Request.IsHttps
            };

            var json = JsonSerializer.Serialize(cart.Items);
            context.Response.Cookies.Append(CartCookieKey, json, cookieOptions);
        }
    }
}
