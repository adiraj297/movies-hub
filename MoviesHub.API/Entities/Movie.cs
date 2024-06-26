using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoviesHub.Entities;

public class Movie
{

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int id { get; set; } 
    
    [Required]
    [Column(TypeName = "varchar(128)")]
    public string title { get; set; }
    
    [Column(TypeName = "date")]
    public DateOnly releaseDate { get; set; }
    
    [Column(TypeName = "varchar(64)")]
    public string genre { get; set; }
    
    public int runtime { get; set; }
    
    public string synopsis { get; set; }
    
    [Column(TypeName = "varchar(64)")]
    public string director { get; set; }
    
    [Column(TypeName = "varchar(8)")]
    public string rating { get; set; }
    
    [Column(TypeName = "varchar(16)")]
    public string princessTheatreMovieId { get; set; }

    public ICollection<MovieCinema> movieCinemas { get; set; } 
        = new List<MovieCinema>();
    public ICollection<MovieReview> movieReviews { get; set; } 
        = new List<MovieReview>();
    
    [NotMapped]
    public string averageScore => movieReviews.Any() ? movieReviews.Select(reviews => reviews.score)
            .Average().ToString("0.00") : "Not yet rated";
    
}