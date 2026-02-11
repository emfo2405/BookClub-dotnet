using System.ComponentModel.DataAnnotations;

namespace BookClub.Models
{
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