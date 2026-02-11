using System.ComponentModel.DataAnnotations;
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
        public int BookModelId { get; set; }
        public BookModel? Book { get; set; }
        [Required]
        public string UserId { get; set; }
        public IdentityUser User { get; set; }

        
    }
}