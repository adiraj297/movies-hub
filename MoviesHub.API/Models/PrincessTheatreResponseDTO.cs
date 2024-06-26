namespace MoviesHub.Models;

public class PrincessTheatreResponseDTO
{
    public string provider { get; set; }
    public IEnumerable<PrincessMovies> movies { get; set; }
}