using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoviesHub.Entities;

public class MovieCinema
{

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int id { get; set; } 
    
    [ForeignKey("movieId")]
    public Movie? movie { get; set; }
    
    public int movieId { get; set; } 
    
    [ForeignKey("cinemaId")]
    public Cinema? cinema { get; set; } 
    
    public int cinemaId { get; set; } 
    
    [Column(TypeName = "date")]
    public DateOnly showTime { get; set; }
    
    [Column(TypeName = "decimal(4, 2)")]
    public decimal ticketPrice { get; set; }
    
}