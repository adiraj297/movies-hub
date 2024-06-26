using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoviesHub.Entities;

public class MovieReview
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int id { get; set; } 
    
    [ForeignKey("movieId")]
    public Movie? movie { get; set; }
    
    public int movieId { get; set; } 
    
    [Column(TypeName = "decimal(4, 2)")]
    public decimal score { get; set; }

    public string? comment { get; set; }
    
    [Column(TypeName = "datetime")]
    public DateTime reviewDate { get; set; } = DateTime.Now;

    [NotMapped]
    public string formattedReviewDate => reviewDate.ToString("dd-MM-yyyy HH:mm:ss");
}