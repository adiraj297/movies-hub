using Microsoft.EntityFrameworkCore;
using MoviesHub.Database;
using MoviesHub.Entities;

namespace MoviesHub.Repositories;

public class MovieHubRepository: IMovieHubRepository
{
    private readonly MovieHubContext _context;

    public MovieHubRepository(MovieHubContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    
    
    private async Task<IEnumerable<Movie>> GetMoviesAsync()
    {
        return await _context.Movie.Include(movie => movie.movieReviews)
            .OrderBy(movie => movie.id).ToListAsync();
    }

    public async Task<Movie?> GetMovieAsync(int movieId)
    {
        return await _context.Movie.Where(movie => movie.id == movieId).FirstOrDefaultAsync();
    }

    public async Task<MovieReview?> GetReviewForMovieAsync(int reviewId)
    {
        return await _context.MovieReview.Where(movieReview => movieReview.id == reviewId).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Movie>> GetMoviesAsync(string? movieTitle, string? genre)
    {
        if (string.IsNullOrEmpty(movieTitle) && string.IsNullOrEmpty(genre))
        {
            return await GetMoviesAsync();
        }

        var movieCollection = _context.Movie as IQueryable<Movie>; //deferred execution

        if (!string.IsNullOrWhiteSpace(movieTitle))
        {
            movieTitle = movieTitle.Trim().ToLower();
            movieCollection = movieCollection.Where(movie => movie.title.ToLower() == movieTitle);
        }
        
        if (!string.IsNullOrWhiteSpace(genre))
        {
            genre = genre.Trim().ToLower();
            movieCollection = movieCollection.Where(movie => 
                movie.genre.ToLower().Contains(genre));
        }

        movieCollection.Include(movie => movie.movieReviews);
        
        return await movieCollection.OrderBy(movie => movie.id).ToListAsync();

    }

    public async Task<Movie?> GetMovieWithAdditionalDetailsAsync(int movieId)
    {
        return await _context.Movie
            .Include(movie => movie.movieReviews)
            .Include(movie => movie.movieCinemas)
            .ThenInclude(movieCinema => movieCinema.cinema)
            .Where(movie => movie.id == movieId).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<MovieReview>> GetReviewsForMovieAsync(int movieId)
    {
        return await _context.MovieReview.Where(movieReview => movieReview.movieId == movieId).ToListAsync();
    }

    public async Task CreateReviewForMovieAsync(int movieId, MovieReview movieReview)
    {
        var movie = await GetMovieAsync(movieId);
        movie?.movieReviews.Add(movieReview);
    }

    public void DeleteReviewForMovie(MovieReview movieReview)
    {
        _context.MovieReview.Remove(movieReview);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() >= 0;
    }

    public async Task<bool> MovieExistsAsync(int movieId)
    {
        return await _context.Movie.AnyAsync(movie => movie.id == movieId);
    }
}