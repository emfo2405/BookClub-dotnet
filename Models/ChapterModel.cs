namespace BookClub.Models
{
    public class ChapterModel
    {
        //Properties
        public int Id { get; set; }
        public string? Title { get; set; }
        public int? Number { get; set; }

        public int BookId { get; set; }
        public BookModel? Book { get; set; }

        
    }
}