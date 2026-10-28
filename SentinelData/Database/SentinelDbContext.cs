using Microsoft.EntityFrameworkCore;
using SentinelData.Database.Models;

namespace SentinelData.Database;

public class SentinelDbContext(DbContextOptions<SentinelDbContext> options) : DbContext(options)
{
    public DbSet<AssetDto> Assets { get; init; }
    
    public DbSet<AssetTypeUrlDto> AssetTypeUrls { get; init; }
    
    public DbSet<AssetTypeUrlResponseDto> AssetTypeUrlResponses { get; init; }
    
    public DbSet<ProjectDto> Projects { get; init; }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<AssetType>()
            .HaveConversion<string>()
            .HaveMaxLength(32);
    }
}