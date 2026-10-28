namespace SentinelDashboard;

public static class Program
{
    /// <summary>
    /// Init all the things...
    /// </summary>
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();

        var app = builder.Build();

        app.MapControllers();

        await app.RunAsync();
    }
}