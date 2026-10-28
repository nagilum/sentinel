using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SentinelData.Database;
using SentinelData.Database.Models;

namespace SentinelData.Extenders;

public static class DatabaseInitializer
{
    extension(IHostApplicationBuilder builder)
    {
        public void AddSentinelDatabaseContextFactory()
        {
            var connectionString = builder.Configuration.GetConnectionString("Sentinel")
                                   ?? "Data Source=sentinel.db";
            
            builder.Services.AddDbContextFactory<SentinelDbContext>(options =>
            {
                options.UseSqlite(connectionString);
            });
        }
    }

    extension(IHost host)
    {
        public async Task AddSentinelTestData()
        {
            var factory = host.Services.GetRequiredService<IDbContextFactory<SentinelDbContext>>();                         
            await using var db = await factory.CreateDbContextAsync();

            var urls = new[]
            {
                "https://example.com"
            };

            foreach (var url in urls)
            {
                if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
                {
                    continue;
                }

                var typeUrl = await db.AssetTypeUrls
                                  .FirstOrDefaultAsync(n => n.Url == uri)
                              ?? new AssetTypeUrlDto
                              {
                                  Url = uri
                              };

                if (typeUrl.Id is 0)
                {
                    await db.AssetTypeUrls.AddAsync(typeUrl);
                    await db.SaveChangesAsync();
                }

                var asset = await db.Assets
                                .FirstOrDefaultAsync(n => n.AssetTypeId == typeUrl.Id);

                if (asset is not null)
                {
                    continue;
                }

                asset = new AssetDto
                {
                    AssetType = AssetType.Url,
                    AssetTypeId = typeUrl.Id
                };
                
                await db.Assets.AddAsync(asset);
                await db.SaveChangesAsync();
            }
        }
        
        public async Task InitializeSentinelDatabase()
        {
            var factory = host.Services.GetRequiredService<IDbContextFactory<SentinelDbContext>>();                         
            await using var db = await factory.CreateDbContextAsync();                                                      
            await db.Database.EnsureCreatedAsync();
        }
    }
}