namespace BookClub.Models;
    //Modell för hur en Moderation för AI bedömning ska struktureras
    //Required-fält måste fyllas i
public class Moderation
{
    public bool IsAllowed {get; set;}
    public string Motivation {get; set;} = string.Empty;
}