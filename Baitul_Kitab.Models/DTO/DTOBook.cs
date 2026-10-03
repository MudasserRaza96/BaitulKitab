using System;

namespace Baitul_Kitab.Models.DTO
{
    public class DTOBook : BaseDTO
    {
        public Guid? Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public Guid? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public Guid? AuthorId { get; set; }
        public string? AuthorName { get; set; }
        public Guid? LanguageId { get; set; }
        public string? LanguageName { get; set; }
        public string? ISBN { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public int? Quantity { get; set; }
        public string? Publisher { get; set; }
        public DateTime? PublishDate { get; set; }
        public int? Pages { get; set; }
        public string? BookCoverImage { get; set; }
        public string? PdfFilePath { get; set; }
        public bool IsFeatured { get; set; }
    }
}
