namespace BookClub.Models;

public class Moderation
{
    public bool IsAllowed {get; set;}
    public string Motivation {get; set;} = string.Empty;
}