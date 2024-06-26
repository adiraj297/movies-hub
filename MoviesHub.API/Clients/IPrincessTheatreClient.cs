using MoviesHub.Models;

namespace MoviesHub.Clients;

public interface IPrincessTheatreClient
{
    Task<PrincessTheatreResponseDTO?> GetMoviesForProvider(string provider);
}