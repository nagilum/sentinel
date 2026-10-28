using System.Linq.Expressions;
using SentinelData.Database.Models;

namespace SentinelData.Services.Data;

public interface ISentinelDataService
{
    /// <summary>
    /// Get a single asset type URL entry.
    /// </summary>
    /// <param name="id">ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Asset type, if found.</returns>
    Task<AssetTypeUrlDto?> GetAssetTypeUrl(int id, CancellationToken cancellationToken);
    
    /// <summary>
    /// Get a list of all assets that has not been scanned or are queued for a next scan.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of assets.</returns>
    Task<AssetDto[]> GetAssetQueue(CancellationToken cancellationToken);

    /// <summary>
    /// Save asset changes.
    /// </summary>
    /// <param name="asset">Asset.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns></returns>
    Task SaveAsset(AssetDto asset, CancellationToken cancellationToken);

    /// <summary>
    /// Save URL response data.
    /// </summary>
    /// <param name="responseData">Response data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task SaveAssetTypeUrlResponse(AssetTypeUrlResponseDto responseData, CancellationToken cancellationToken);
}