using System;
using System.Collections.Generic;

namespace Baitul_Kitab.Models.DTO.UserStore
{
    /// <summary>Book summary shown in the user-facing grid.</summary>
    public class UserBookCardDTO
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? AuthorName { get; set; }
        public string? CategoryName { get; set; }
        public string? LanguageName { get; set; }
        public string? BookCoverImage { get; set; }
        public string? PdfFilePath { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public int? Quantity { get; set; }

        public bool HasDiscount => DiscountPrice.HasValue && DiscountPrice.Value > 0 && DiscountPrice.Value < Price;
        public decimal EffectivePrice => HasDiscount ? DiscountPrice!.Value : Price;
        public bool InStock => Quantity.GetValueOrDefault() > 0;
    }

    /// <summary>Full book information for the details page.</summary>
    public class UserBookDetailDTO : UserBookCardDTO
    {
        public string? Description { get; set; }
        public string? ISBN { get; set; }
        public string? Publisher { get; set; }
        public DateTime? PublishDate { get; set; }
        public int? Pages { get; set; }
        public string? PdfFilePath { get; set; }
    }

    /// <summary>Lookup item for the Category / Author / Language filters.</summary>
    public class UserFilterOptionDTO
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
    }

    /// <summary>Search, filter and paging input of the Books page.</summary>
    public class UserBookFilterDTO
    {
        public string? Search { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? AuthorId { get; set; }
        public Guid? LanguageId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int Page { get; set; } = 1;
    }

    /// <summary>Result model of the Books page.</summary>
    public class UserBookListDTO
    {
        public UserBookFilterDTO Filter { get; set; } = new();
        public List<UserBookCardDTO> Books { get; set; } = new();
        public List<UserFilterOptionDTO> Categories { get; set; } = new();
        public List<UserFilterOptionDTO> Authors { get; set; } = new();
        public List<UserFilterOptionDTO> Languages { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int Page { get; set; } = 1;
        public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
    }
}
