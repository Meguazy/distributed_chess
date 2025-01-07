using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Orleans.Hosting;
using Microsoft.EntityFrameworkCore;
using Orleans;
using VaultSharp;
using VaultSharp.V1.AuthMethods;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp.V1.Commons;
using System.Threading.Tasks;
using System;
using ChessSilo.Persistence;
using StackExchange.Redis;

namespace ChessSilo
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // Initialize one of the several auth methods.
            IAuthMethodInfo authMethod = new TokenAuthMethodInfo("testtoken");

            // Initialize settings. You can also set proxies, custom delegates etc. here.
            var vaultClientSettings = new VaultClientSettings("http://localhost:8200", authMethod);

            IVaultClient vaultClient = new VaultClient(vaultClientSettings);
            var dbUsername = "";
            var dbPassword = "";
            try
            {
                // Fetch the secret from the KV Secrets Engine (v2)
                Secret<SecretData> secret = await vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync(
                    path: "configs",
                    mountPoint: "database"
                );

                // Retrieve values from the secret
                dbUsername = secret.Data.Data["username"].ToString();
                dbPassword = secret.Data.Data["password"].ToString();

                Console.WriteLine($"Database Username: {dbUsername}");
                Console.WriteLine($"Database Password: {dbPassword}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching secrets: {ex.Message}");
            }

            // Build the connection string dynamically
            var connectionString = $"Server=localhost,1433;Database=EventBus;User Id={dbUsername};Password={dbPassword};TrustServerCertificate=true";

            // Create and run the web application
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container
            builder.Services.AddDbContext<GamesContext>(options =>
                options.UseSqlServer(connectionString)
            );
            
            // Add services for controllers
            builder.Services.AddControllers();

            // Add services for cache invalidation
            builder.Services.AddSingleton<CacheInvalidationService>();

            Console.WriteLine("Connecting to Redis...");
            // Add services for Redis
            // Redis Configuration
            var redisConfiguration = new ConfigurationOptions
            {
                EndPoints = { "localhost:6379" }, // Docker service name and port
                AbortOnConnectFail = false,  // Retry if the connection fails initially
                AllowAdmin = true,           // Enable administrative commands if needed
                ConnectRetry = 3,            // Retry attempts
                KeepAlive = 180              // Prevent connection from timing out
            };

            // Connect to Redis
            var redis = ConnectionMultiplexer.Connect(redisConfiguration);
            builder.Services.AddSingleton<IConnectionMultiplexer>(redis);
            builder.Services.AddSingleton<IDatabase>(sp => sp.GetRequiredService<IConnectionMultiplexer>().GetDatabase());

            Console.WriteLine("Connected to Redis.");

            // Add hosted service for game cleanup
            builder.Services.AddHostedService<GameCleanupService>();

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

            // Create database
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<GamesContext>();
                db.Database.EnsureCreated();
            }

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

public class GamesContext : DbContext
{
    public DbSet<Game> Games { get; set; } = default!;
    public GamesContext(DbContextOptions<GamesContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new GameConfiguration());
    }
}