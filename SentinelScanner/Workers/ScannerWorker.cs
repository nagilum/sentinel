using System.Diagnostics;
using System.Text.Json;
using SentinelData;
using SentinelData.Database.Models;
using SentinelData.Services.Data;

namespace SentinelScanner.Workers;

public class ScannerWorker(
    IHttpClientFactory httpClientFactory,
    ILogger<ScannerWorker> logger,
    ISentinelDataService sentinelDataService
    ) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var interval = TimeSpan.FromSeconds(30);

        while (!cancellationToken.IsCancellationRequested)
        {
            await this.ScanAssets(cancellationToken);
            await Task.Delay(interval, cancellationToken);
        }
    }

    private async Task ScanAssets(CancellationToken cancellationToken)
    {
        var assets = await sentinelDataService.GetAssetQueue(cancellationToken);

        await Parallel.ForEachAsync(
            assets,
            cancellationToken,
            this.ScanAsset);
    }

    private async ValueTask ScanAsset(AssetDto asset, CancellationToken cancellationToken)
    {
        try
        {
            var assetType = asset.AssetType switch
            {
                AssetType.Url => await sentinelDataService.GetAssetTypeUrl(asset.AssetTypeId, cancellationToken),
                _ => throw new InvalidCastException($"Asset type {asset.AssetType} not implemented")
            };

            if (assetType is null)
            {
                throw new Exception($"Asset type {asset.AssetType} {asset.AssetTypeId} not found.");
            }
        
            switch (asset.AssetType)
            {
                case AssetType.Url:
                    await this.ScanAssetTypeUrl(asset, assetType, cancellationToken);
                    break;
            
                default:
                    throw new InvalidCastException($"Asset type {asset.AssetType} not implemented");
            }

            asset.Updated = DateTime.UtcNow;
            asset.LastScan = DateTime.UtcNow;
            asset.NextScan = DateTime.UtcNow.AddMinutes(3);

            await sentinelDataService.SaveAsset(
                asset,
                cancellationToken.IsCancellationRequested
                    ? CancellationToken.None
                    : cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occured during asset {id} scan.", asset.Id);
        }
    }

    private async Task ScanAssetTypeUrl(
        AssetDto asset,
        AssetTypeUrlDto url,
        CancellationToken cancellationToken)
    {
        var urlResponse = new AssetTypeUrlResponseDto
        {
            AssetId = asset.Id
        };

        try
        {
            var watch = Stopwatch.StartNew();
            var client = httpClientFactory.CreateClient("SentinelScanner");

            using var res = await client.GetAsync(url.Url, cancellationToken);

            watch.Stop();

            urlResponse.StatusCode = (int)res.StatusCode;
            urlResponse.StatusDescription = res.ReasonPhrase?[..Math.Min(res.ReasonPhrase.Length, 64)];
            urlResponse.TimeMilliseconds = watch.ElapsedMilliseconds;
            urlResponse.Timeout = false;
            urlResponse.SslError = false;

            urlResponse.Size = res.Content.Headers.ContentLength;

            if (urlResponse.Size is null)
            {
                var bytes = await res.Content.ReadAsByteArrayAsync(cancellationToken);
                urlResponse.Size = bytes.Length;
            }

            var headers = new Dictionary<string, string?>();

            foreach (var (key, value) in res.Headers)
            {
                headers.TryAdd(key, value.FirstOrDefault());
            }

            foreach (var (key, value) in res.Content.Headers)
            {
                headers.TryAdd(key, value.FirstOrDefault());
            }

            foreach (var (key, value) in res.TrailingHeaders)
            {
                headers.TryAdd(key, value.FirstOrDefault());
            }

            using var stream = new MemoryStream();
            await JsonSerializer.SerializeAsync(stream, headers, JsonSerializerOptions.Default, cancellationToken);
            stream.Position = 0;
            var json = await new StreamReader(stream).ReadToEndAsync(cancellationToken);

            urlResponse.Headers = json;
        }
        catch (TaskCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            urlResponse.Timeout = true;
        }
        catch (HttpRequestException ex) when (ex.HttpRequestError is HttpRequestError.SecureConnectionError)
        {
            urlResponse.SslError = true;

            urlResponse.ErrorType = "SslError";
            urlResponse.ErrorMessage = ex.Message[..Math.Min(ex.Message.Length, 256)];
        }
        catch (Exception ex) when (!cancellationToken.IsCancellationRequested)
        {
            var errorType = ex.GetType().Name;

            urlResponse.ErrorType = errorType[..Math.Min(errorType.Length, 64)];
            urlResponse.ErrorMessage = ex.Message[..Math.Min(ex.Message.Length, 256)];
        }

        await sentinelDataService.SaveAssetTypeUrlResponse(
            urlResponse,
            cancellationToken.IsCancellationRequested
                ? CancellationToken.None
                : cancellationToken);
    }
}