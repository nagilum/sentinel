namespace SentinelData.Database.Models;

public interface IProjectDto
{
    int Id { get; init; }
    
    DateTime Created { get; init; }
    
    DateTime Updated { get; set; }
    
    string? Name { get; set; }
}