using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SentinelData.Database.Models;

[Table("AssetTypeUrlResponses")]
[Index(nameof(AssetId))]
public class AssetTypeUrlResponseDto : IAssetTypeUrlResponseDto
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column]
    public int Id { get; init; }
    
    [Column]
    public required int AssetId { get; init; }
    
    [Column]
    public DateTime Created { get; init; } = DateTime.UtcNow;

    [Column]
    public int? StatusCode { get; set; }
    
    [Column]
    [MaxLength(64)]
    public string? StatusDescription { get; set; }

    [Column]
    public long? TimeMilliseconds { get; set; }

    [Column]
    public long? Size { get; set; }

    [Column]
    [MaxLength(512)]
    public string? Headers { get; set; }

    [Column]
    public bool? Timeout { get; set; }

    [Column]
    [MaxLength(64)]
    public string? ErrorType { get; set; }
    
    [Column]
    [MaxLength(256)]
    public string? ErrorMessage { get; set; }
}