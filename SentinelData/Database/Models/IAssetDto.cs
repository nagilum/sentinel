namespace SentinelData.Database.Models;

public interface IAssetDto
{
    int Id { get; init; }
    
    int? ProjectId { get; set; }
    
    DateTime Created { get; init; }
    
    DateTime Updated { get; set; }
    
    DateTime? LastScan { get; set; }
    
    DateTime? NextScan { get; set; }
    
    AssetType AssetType { get; set; }
    
    int AssetTypeId { get; set; }
    
    string? Name { get; set; }
}