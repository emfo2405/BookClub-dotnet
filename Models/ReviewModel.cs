using Microsoft.AspNetCore.Identity;

namespace BookClub.Models
{
    public class ReviewModel
    {
        //Properties
        public int Id { get; set; }
        public int? Review { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int BookModelId { get; set; }
        public BookModel? Book { get; set; }

        public string UserId { get; set; }
        public IdentityUser User { get; set; }

        
    }
}