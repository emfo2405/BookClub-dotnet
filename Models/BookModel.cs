using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookClub.Models
{
    public class BookModel
    {
        //Properties
        public int Id { get; set; }
        [Required]
        public string? Title { get; set; }
        [Required]
        public int? Year { get; set; }
        [Required]
        public string? Description { get; set; }
        [Url]
        public string? Image { get; set; }
        [Required]
        [Display(Name = "Author")]
        [ForeignKey("Author")]
        public int AuthorModelId { get; set; }
        public AuthorModel? Author { get; set; }
        
        public ICollection<ChapterModel> Chapters {get; set;} = new List<ChapterModel>();
        public ICollection<ReviewModel> Reviews {get; set;} = new List<ReviewModel>();
    }
}