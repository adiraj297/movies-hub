namespace MoviesHub.Models;

public class MovieCinemaDTO
{
    public string name { get; set; }
    public string location { get; set; }
    public DateOnly showTime { get; set; }
    public decimal ticketPrice { get; set; }
   
}