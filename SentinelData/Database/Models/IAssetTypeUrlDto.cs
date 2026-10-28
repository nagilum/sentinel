namespace SentinelData.Database.Models;

public interface IAssetTypeUrlDto
{
    int Id { get; init; }
    
    DateTime Created { get; init; }
    
    DateTime Updated { get; set; }
    
    Uri Url { get; set; }
}