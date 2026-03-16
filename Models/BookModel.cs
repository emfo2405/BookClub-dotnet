using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookClub.Models
{
    //Modell för hur en bok ska struktureras
    //Required-fält måste fyllas i
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

        //Kortare version av description som kan visas på startsidan
        public string ShortDescription => 
        string.IsNullOrEmpty(Description)
        ? ""
        : Description.Length > 250 
            ? Description.Substring(0, 250) + "..."
            : Description; 
        [Url]
        public string? Image { get; set; }
        //Koppling till författare
        [Required]
        [Display(Name = "Author")]
        [ForeignKey("Author")]
        public int AuthorModelId { get; set; }
        public AuthorModel? Author { get; set; }
        //Koppling till kapitel, varje bok kan ha många kapitel
        public ICollection<ChapterModel> Chapters {get; set;} = new List<ChapterModel>();
        //Koppling till recensioner, varje bok kan ha många recensioner
        public ICollection<ReviewModel> Reviews {get; set;} = new List<ReviewModel>();
        //Medelbetyget räknas ut men sparas inte i databas
        [NotMapped]
        public double? AverageRating =>
        Reviews != null && Reviews.Any(r => r.Review.HasValue)
        ? Reviews.Where(r => r.Review.HasValue).Average(r => r.Review.Value)
        : null;
    }
}