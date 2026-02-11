using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookClub.Models
{
    public class ChapterModel
    {
        //Properties
        public int Id { get; set; }
        public string? Title { get; set; }
        [Required]
        public int? Number { get; set; }
        [Required]
        [Display(Name = "Book")]
        [ForeignKey("Book")]
        public int BookModelId { get; set; }
        public BookModel? Book { get; set; }

        
    }
}