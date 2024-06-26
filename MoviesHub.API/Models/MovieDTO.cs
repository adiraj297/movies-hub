namespace MoviesHub.Models;

public class MovieDTO
{
    public int id { get; set; } 
    public string title { get; set; }
    public DateOnly releaseDate { get; set; }
    public string genre { get; set; }
    public int runtime { get; set; }
    public string synopsis { get; set; }
    public string director { get; set; }
    public string rating { get; set; }
    public string averageScore { get; set; }
}