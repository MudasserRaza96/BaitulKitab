using System;

namespace Baitul_Kitab.Models.DTO
{
    public class DTOAuthor : BaseDTO
    {
        public Guid? Id { get; set; }
        public string? Name { get; set; }
        public string? Biography { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Nationality { get; set; }
    }
}
