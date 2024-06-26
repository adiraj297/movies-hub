using MoviesHub.Clients;
using MoviesHub.Models;

namespace MoviesHub.Services;

public class PriceFetchService
{
    private readonly IPrincessTheatreClient _princessTheatreClient ;
        
    public PriceFetchService(IPrincessTheatreClient princessTheatreClient)
    {
        _princessTheatreClient = princessTheatreClient;
    }

    public async Task<MovieWithMovieCinemaDTO> AllocatePrincessPricesForMovie(
        string princessTheatreMovieId,
        MovieWithMovieCinemaDTO movieWithMovieCinemaDto
        )
    {
        movieWithMovieCinemaDto.cinemaWorldTicketPrice = await FetchCinemaProviderPrice(CinemaProvider.cw, princessTheatreMovieId);
        movieWithMovieCinemaDto.filmWorldTicketPrice = await FetchCinemaProviderPrice(CinemaProvider.fw, princessTheatreMovieId);
        return movieWithMovieCinemaDto;
    }

    private async Task<decimal?> FetchCinemaProviderPrice(CinemaProvider cinemaProvider, string princessTheatreMovieId)
    {
        var provider = cinemaProvider == CinemaProvider.cw ? "cinemaworld" : "filmworld";
        var providerMovies = await _princessTheatreClient.GetMoviesForProvider(provider);
        return providerMovies?.movies.FirstOrDefault(movie => movie.id == $"{cinemaProvider+princessTheatreMovieId}")?.price;
    }
}