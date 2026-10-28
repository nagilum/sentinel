using SentinelData.Extenders;
using SentinelData.Services.Data;
using SentinelScanner.Workers;

namespace SentinelScanner;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        builder.AddSentinelDatabaseContextFactory();

        builder.Services.AddHttpClient("SentinelScanner", client =>
            {
                client.Timeout = TimeSpan.FromSeconds(15);
                client.DefaultRequestHeaders.UserAgent.ParseAdd("SentinelScanner/1.0");
            })
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                AllowAutoRedirect = false,
                MaxConnectionsPerServer = 10
            });
        
        builder.Services.AddSingleton<ISentinelDataService, SentinelDataService>();
        
        builder.Services.AddHostedService<ScannerWorker>();

        var host = builder.Build();

        await host.InitializeSentinelDatabase();
        await host.AddSentinelTestData();
        await host.RunAsync();
    }
}