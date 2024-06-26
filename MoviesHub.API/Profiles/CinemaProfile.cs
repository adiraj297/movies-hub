using AutoMapper;

namespace MoviesHub.Profiles;

public class CinemaProfile: Profile
{
    public CinemaProfile()
    {
        CreateMap<Entities.Cinema, Models.CinemaDTO>();
    }
    
}