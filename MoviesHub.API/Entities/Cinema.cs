using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoviesHub.Entities;

public class Cinema
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int id { get; set; } 
    
    [Required]
    [Column(TypeName = "varchar(64)")]
    public string name { get; set; }
    
    [MaxLength(250)]
    public string location { get; set; }
}