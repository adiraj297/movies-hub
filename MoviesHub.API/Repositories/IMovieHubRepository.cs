using MoviesHub.Entities;

namespace MoviesHub.Repositories;

public interface IMovieHubRepository
{
    Task<IEnumerable<Movie>> GetMoviesAsync(string? movieTitle, string? genre);
    Task<Movie?> GetMovieWithAdditionalDetailsAsync(int movieId);
    Task<MovieReview?> GetReviewForMovieAsync(int reviewId);
    Task<IEnumerable<MovieReview>> GetReviewsForMovieAsync(int movieId);
    Task CreateReviewForMovieAsync(int movieId, MovieReview movieReview);

    void DeleteReviewForMovie(MovieReview movieReview);

    Task<bool> SaveChangesAsync();
    Task<bool> MovieExistsAsync(int movieId);

}