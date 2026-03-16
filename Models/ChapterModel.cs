using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookClub.Models
{
    //Modell för hur ett kapitel ska struktureras
    //Required-fält måste fyllas i
    public class ChapterModel
    {
        //Properties
        public int Id { get; set; }
        public string? Title { get; set; }
        [Required]
        public int? Number { get; set; }
        //Koppling till bok
        [Required]
        [Display(Name = "Book")]
        [ForeignKey("Book")]
        public int BookModelId { get; set; }
        public BookModel? Book { get; set; }
        //Varje kapitel kan ha flera diskussionsinlägg
        public ICollection<DiscussionModel> Discussions {get; set;} = new List<DiscussionModel>();
        
    }
}