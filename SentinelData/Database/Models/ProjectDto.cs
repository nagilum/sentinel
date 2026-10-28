using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SentinelData.Database.Models;

[Table("Projects")]
public class ProjectDto : IProjectDto
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column]
    public int Id { get; init; }

    [Column]
    public DateTime Created { get; init; } = DateTime.UtcNow;
    
    [Column]
    public DateTime Updated { get; set; } = DateTime.UtcNow;
    
    [Column]
    [MaxLength(64)]
    public string? Name { get; set; }
}