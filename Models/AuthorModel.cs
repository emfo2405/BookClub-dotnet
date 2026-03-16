using System.ComponentModel.DataAnnotations;

namespace BookClub.Models
{
    //Modell för hur en författare ska struktureras
    //Required-fält måste fyllas i
    public class AuthorModel
    {
        //Properties
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        public string? Description { get; set; }
        [Required]
        public string? Country { get; set; }

        
    }
}