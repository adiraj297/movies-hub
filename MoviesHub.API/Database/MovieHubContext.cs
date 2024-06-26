using Microsoft.EntityFrameworkCore;
using MoviesHub.Entities;

namespace MoviesHub.Database;

public class MovieHubContext : DbContext
{
    public DbSet<Movie> Movie { get; set; }
    public DbSet<MovieCinema> MovieCinema { get; set; }
    public DbSet<Cinema> Cinema { get; set; }
    public DbSet<MovieReview> MovieReview { get; set; }
    
    public MovieHubContext(DbContextOptions<MovieHubContext> options) 
        :base(options){}
}