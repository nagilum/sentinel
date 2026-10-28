using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SentinelData.Database;
using SentinelData.Database.Models;

namespace SentinelData.Services.Data;

public class SentinelDataService(
    IDbContextFactory<SentinelDbContext> dbContextFactory,
    ILogger<SentinelDataService> logger
    ) : ISentinelDataService
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public async Task<AssetTypeUrlDto?> GetAssetTypeUrl(int id, CancellationToken cancellationToken)
    {
        try
        {
            await using var db = await dbContextFactory.CreateDbContextAsync(cancellationToken);

            var entry = await db.AssetTypeUrls
                .AsNoTracking()
                .FirstOrDefaultAsync(n => n.Id == id, cancellationToken);
            
            return entry;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while getting asset type url {id}.", id);
            return null;
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public async Task<AssetDto[]> GetAssetQueue(CancellationToken cancellationToken)
    {
        try
        {
            await using var db = await dbContextFactory.CreateDbContextAsync(cancellationToken);

            var list = await db.Assets
                .AsNoTracking()
                .Where(n => n.NextScan == null ||
                            n.NextScan <= DateTime.UtcNow)
                .ToArrayAsync(cancellationToken);
            
            return list;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while getting asset queue.");
            return [];
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public async Task SaveAsset(AssetDto asset, CancellationToken cancellationToken)
    {
        try
        {
            await using var db = await dbContextFactory.CreateDbContextAsync(cancellationToken);

            if (asset.Id is 0)
            {
                await db.Assets.AddAsync(asset, cancellationToken);
            }
            else
            {
                var entry = db.Attach(asset);
                entry.State = EntityState.Modified;
            }
            
            await db.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while saving asset {id}.", asset.Id);
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public async Task SaveAssetTypeUrlResponse(AssetTypeUrlResponseDto responseData, CancellationToken cancellationToken)
    {
        try
        {
            await using var db = await dbContextFactory.CreateDbContextAsync(cancellationToken);

            if (responseData.Id is 0)
            {
                await db.AssetTypeUrlResponses.AddAsync(responseData, cancellationToken);
            }
            else
            {
                var entry = db.Attach(responseData);
                entry.State = EntityState.Modified;
            }
            
            await db.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while saving asset type url response for asset {id}.", responseData.AssetId);
        }
    }
}