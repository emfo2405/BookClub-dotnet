namespace BookClub.Models;
    //Modell för hur ett error-meddelande ska struktureras
public class ErrorViewModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
