namespace BookClub.Models
{
    public class HomeModel
    {
        public BookModel? CurrentBook {get; set;}
        public List<BookModel> TopRated {get; set;} = new();
    }
}