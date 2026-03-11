using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace BookClub.Models
{
    public class ReviewModel
    {
        //Properties
        public int Id { get; set; }
        [Required]
        [Range(1, 10, ErrorMessage ="Betyget måste vara mellan 1 och 10")]
        public int? Review { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [Required]
        [Display(Name = "Book")]
        [ForeignKey("Book")]
        public int BookModelId { get; set; }
        public BookModel? Book { get; set; }
        public string? UserName { get; set; } 
        public string? UserId { get; set; }

        
    }
}