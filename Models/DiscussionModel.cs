    using Microsoft.AspNetCore.Identity;

namespace BookClub.Models
{
    public class DiscussionModel
    {
        //Properties
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int ChapterModelId { get; set; }
        public ChapterModel? Chapter { get; set; }

        public string UserId { get; set; }
        public IdentityUser User { get; set; }

        
    }
}