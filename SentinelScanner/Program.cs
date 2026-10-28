namespace SentinelScanner;

public static class Program
{
    /// <summary>
    /// Init all the things...
    /// </summary>
    public static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        
        //builder.Services.AddHostedService<Worker>();

        var host = builder.Build();
        
        await host.RunAsync();
    }
}