using Baitul_Kitab.Data.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Baitul_Kitab.Models
{
    public class Book : BaseModel
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(200)]
        public string? Title { get; set; }

        [StringLength(2000)]
        public string? Description { get; set; }

        public Guid? CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }

        public Guid? AuthorId { get; set; }
        [ForeignKey("AuthorId")]
        public Author? Author { get; set; }

        public Guid? LanguageId { get; set; }
        [ForeignKey("LanguageId")]
        public Language? Language { get; set; }

        [StringLength(20)]
        public string? ISBN { get; set; }

        [Range(0, 999999)]
        public decimal Price { get; set; }

        [Range(0, 999999)]
        public decimal? DiscountPrice { get; set; }

        public int? Quantity { get; set; }

        [StringLength(200)]
        public string? Publisher { get; set; }

        public DateTime? PublishDate { get; set; }

        public int? Pages { get; set; }

        [StringLength(500)]
        public string? BookCoverImage { get; set; }

        [StringLength(500)]
        public string? PdfFilePath { get; set; }

        public bool IsFeatured { get; set; } = false;
    }
}
