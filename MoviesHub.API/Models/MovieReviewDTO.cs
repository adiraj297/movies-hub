namespace MoviesHub.Models;

public class MovieReviewDTO
{
    public int id { get; set; } 
    public int movieId { get; set; } 
    public decimal score { get; set; }
    public string? comment { get; set; }
    public string reviewDate { get; set; }
}