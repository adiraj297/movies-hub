using AutoMapper;

namespace MoviesHub.Profiles;

public class MovieReviewProfile:Profile
{
    public MovieReviewProfile()
    {
        CreateMap<Models.MovieReviewCreationDTO, Entities.MovieReview>();
        CreateMap<Entities.MovieReview, Models.MovieReviewDTO>()
            .ForMember(dest => dest.reviewDate, opt 
                => opt.MapFrom(src => src.formattedReviewDate));
        CreateMap<Models.MovieReviewUpdateDTO, Entities.MovieReview>().ReverseMap();
    }
}