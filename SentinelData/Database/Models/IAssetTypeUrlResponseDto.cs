namespace SentinelData.Database.Models;

public interface IAssetTypeUrlResponseDto
{
    int Id { get; init; }
    
    int AssetId { get; init; }
    
    DateTime Created { get; init; }
    
    int? StatusCode { get; set; }
    
    string? StatusDescription { get; set; }
    
    long? TimeMilliseconds { get; set; }
    
    long? Size { get; set; }
    
    string? Headers { get; set; }
    
    bool? Timeout { get; set; }
    
    string? ErrorType { get; set; }
    
    string? ErrorMessage { get; set; }
}