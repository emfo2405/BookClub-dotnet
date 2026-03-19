namespace BookClub.Models
{
    //Modell för hur en home ska struktureras, skapades i controller
    public class HomeModel
    {
        public BookModel? CurrentBook {get; set;}

        public List<BookModel> TopRated {get; set;} = new();
    }
}