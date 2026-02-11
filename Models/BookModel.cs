using System.ComponentModel.DataAnnotations;

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
        public int AuthorModelId { get; set; }
        public AuthorModel? Author { get; set; }
        
    }
}