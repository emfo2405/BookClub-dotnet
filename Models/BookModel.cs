namespace BookClub.Models
{
    public class BookModel
    {
        //Properties
        public int Id { get; set; }
        public string? Title { get; set; }
        public int? Year { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }

        public int AuthorId { get; set; }
        public AuthorModel Author { get; set; }
        
    }
}