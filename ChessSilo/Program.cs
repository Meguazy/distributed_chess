using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Orleans.Hosting;
using Orleans;

namespace ChessSilo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Create and run the web application
            var builder = WebApplication.CreateBuilder(args);

            // Add services for controllers
            builder.Services.AddControllers();

            // Configure Orleans
            builder.Host.UseOrleans(siloBuilder =>
            {
                // Use localhost clustering for Orleans
                siloBuilder.UseLocalhostClustering();

                // Enable Orleans Dashboard and specify port
                siloBuilder.UseDashboard(options =>
                {
                    options.Port = 8080; // Specify the dashboard port
                });
            });

            // Build the application
            var app = builder.Build();

            // Add middleware to the app pipeline
            app.UseHttpsRedirection();
            app.UseAuthorization();

            app.UseStaticFiles();
            
            // Map controllers (endpoints)
            app.MapControllers();

            // Redirect to index.html on root URL
            app.MapGet("/", () => Results.Redirect("/index.html"));

            // Run the application
            app.Run();
        }
    }
}
