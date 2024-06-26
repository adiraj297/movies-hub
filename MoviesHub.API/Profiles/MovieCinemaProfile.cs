using AutoMapper;

namespace MoviesHub.Profiles;

public class MovieCinemaProfile : Profile
{
    public MovieCinemaProfile()
    {
        CreateMap<Entities.MovieCinema, Models.MovieCinemaDTO>()
            .ForMember(dest => dest.name, 
                opt => opt.MapFrom(src => src.cinema.name))
            .ForMember(dest => dest.location, 
            opt => opt.MapFrom(src => src.cinema.location));
    }
}