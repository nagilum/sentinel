using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SentinelData.Database.Models;

[Table("AssetTypeUrls")]
public class AssetTypeUrlDto : IAssetTypeUrlDto
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
    public required Uri Url { get; set; }
}