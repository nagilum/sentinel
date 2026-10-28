using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SentinelData.Database.Models;

[Table("Assets")]
[Index(nameof(ProjectId))]
[Index(nameof(NextScan))]
public class AssetDto : IAssetDto
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column]
    public int Id { get; init; }
    
    [Column]
    public int? ProjectId { get; set; }
    
    [Column]
    public DateTime Created { get; init; } = DateTime.UtcNow;
    
    [Column]
    public DateTime Updated { get; set; } = DateTime.UtcNow;

    [Column]
    public DateTime? LastScan { get; set; }
    
    [Column]
    public DateTime? NextScan { get; set; }

    [Column]
    public AssetType AssetType { get; set; }
    
    [Column]
    public int AssetTypeId { get; set; }
    
    [MaxLength(64)]
    public string? Name { get; set; }
}