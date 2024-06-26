using System.ComponentModel.DataAnnotations;

namespace MoviesHub.Models;

public class MovieReviewCreationDTO
{
    
    [Range(0.01, 10.0, ErrorMessage = "The field score is must be provided in the request body and should be in range between 0.01 and 10.0")] 
    public decimal score { get; set; }
    
    public string? comment { get; set; }
    
}