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
        public int? Review { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [Required]
        [Display(Name = "Book")]
        [ForeignKey("Book")]
        public int BookModelId { get; set; }
        public BookModel? Book { get; set; }
        public string? UserName { get; set; } 

        
    }
}