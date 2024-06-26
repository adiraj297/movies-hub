using AutoMapper;

namespace MoviesHub.Profiles;

public class MovieProfile: Profile
{

    public MovieProfile()
    {
        CreateMap<Entities.Movie, Models.MovieDTO>();
        CreateMap<Entities.Movie, Models.MovieWithMovieCinemaDTO>();
    }
    
}