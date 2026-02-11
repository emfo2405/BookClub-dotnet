using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace BookClub.Models
{
    public class DiscussionModel
    {
        //Properties
        public int Id { get; set; }
        [Required]
        public string? Title { get; set; }
        [Required]
        public string? Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [Required]
        public int ChapterModelId { get; set; }
        public ChapterModel? Chapter { get; set; }
        [Required]
        public string UserId { get; set; }
        public IdentityUser User { get; set; }

        
    }
}