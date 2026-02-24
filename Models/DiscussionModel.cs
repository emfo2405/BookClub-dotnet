using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        [Display(Name = "Chapter")]
        [ForeignKey("Chapter")]
        public int ChapterModelId { get; set; }
        public ChapterModel? Chapter { get; set; }
        public string? UserName { get; set; }   
        public string? UserId {get; set;}
        [ForeignKey("UserId")]
        public IdentityUser? User {get; set;}
    }
}