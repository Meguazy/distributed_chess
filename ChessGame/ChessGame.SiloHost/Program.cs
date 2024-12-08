using Microsoft.Extensions.Hosting;
using Orleans;
using Orleans.Hosting;
using ChessGame.Grains;
using ChessGame.Grains.Interfaces;
using System;
using System.Threading.Tasks;

namespace ChessGame.SiloHost
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // Create and configure the host for the Orleans silo
            var host = Host.CreateDefaultBuilder(args)
                .UseOrleans(builder =>
                {
                    // Set up clustering (for local development)
                    builder.UseLocalhostClustering()
                        .AddMemoryGrainStorage("Default")  // Use memory storage for grains (for development purposes)
                        .AddSimpleMessageStreamProvider("ChessStream"); // Add stream provider if needed

                    // Optionally add persistence providers or other Orleans services if needed
                })
                .ConfigureServices(services =>
                {
                    // Register services, such as dependencies for grains or any additional services
                })
                .Build();

            // Run the host (start the Orleans silo)
            await host.RunAsync();
        }
    }
}
