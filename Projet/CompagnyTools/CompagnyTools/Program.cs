using CompagnyTools;
using CompagnyTools.Context;
using CompagnyTools.Interface;
using CompagnyTools.Services;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllersWithViews();

        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.MachineName}.json", optional: true)
            .Build();

        string dbConnectionString = "Server=192.168.97.206;Port=5432;Database=mistral;User Id=mistral;Password=wb5%r4&VzkM#k9p";

        // Launch connection postgres 
        ServiceProvider serviceProvider = new ServiceCollection()
        .AddDbContext<EFCoreContext>()
        .AddDbContext<Access>(
            options => options.UseNpgsql(dbConnectionString)
        )
        .AddSingleton<IConfiguration>(configuration)
        .BuildServiceProvider();

        CreateWebHostBuilder(args).Build().Run();

    }

    public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
    WebHost.CreateDefaultBuilder(args)
        .UseStartup<Startup>();
}
