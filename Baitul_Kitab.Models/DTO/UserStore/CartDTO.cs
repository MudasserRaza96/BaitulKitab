using System;
using System.Collections.Generic;
using System.Linq;

namespace Baitul_Kitab.Models.DTO.UserStore
{
    public class CartItemDTO
    {
        public Guid BookId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? AuthorName { get; set; }
        public string? CoverImage { get; set; }
        public decimal Price { get; set; }
        public decimal? OriginalPrice { get; set; }
        public int Quantity { get; set; }
        public int StockQuantity { get; set; }

        public bool HasDiscount => OriginalPrice.HasValue && OriginalPrice.Value > Price;
        public decimal Total => Price * Quantity;
    }

    public class CartDTO
    {
        public List<CartItemDTO> Items { get; set; } = new();

        public int TotalItems => Items.Sum(i => i.Quantity);
        public decimal Subtotal => Items.Sum(i => i.Total);
        public decimal ShippingFee => 0m; // Free shipping
        public decimal GrandTotal => Subtotal + ShippingFee;
        public bool IsEmpty => Items.Count == 0;
    }
}
