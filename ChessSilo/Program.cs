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
using DotNetEnv;

namespace ChessSilo
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("=== ChessSilo Application Starting ===");
            Console.WriteLine($"Application started at: {DateTime.Now}");
            Console.WriteLine($"Arguments: {string.Join(" ", args)}");

            // Load environment variables from the .env file
            Console.WriteLine("Loading environment variables from .env file...");
            Env.Load();
            Console.WriteLine("Environment variables loaded successfully.");

            // Fetch the Vault token from the environment variables
            Console.WriteLine("Fetching Vault token from environment variables...");
            string? vaultToken = Environment.GetEnvironmentVariable("VAULT_DEV_ROOT_TOKEN_ID");

            if (string.IsNullOrEmpty(vaultToken))
            {
                Console.WriteLine("ERROR: Vault token is missing from the environment.");
                Console.WriteLine("Please ensure VAULT_DEV_ROOT_TOKEN_ID is set in your environment or .env file.");
                return;
            }
            Console.WriteLine("Vault token retrieved successfully.");

            // Initialize the Vault authentication method
            Console.WriteLine("Initializing Vault authentication...");
            IAuthMethodInfo authMethod = new TokenAuthMethodInfo(vaultToken);
            Console.WriteLine("Vault authentication method initialized.");

            // Initialize settings. You can also set proxies, custom delegates etc. here.
            Console.WriteLine("Configuring Vault client settings...");
            var vaultClientSettings = new VaultClientSettings("http://localhost:8200", authMethod);
            Console.WriteLine("Vault client settings configured for http://localhost:8200");

            Console.WriteLine("Creating Vault client...");
            IVaultClient vaultClient = new VaultClient(vaultClientSettings);
            Console.WriteLine("Vault client created successfully.");

            var dbUsername = "";
            var dbPassword = "";
            Console.WriteLine("Attempting to fetch database credentials from Vault...");
            try
            {
                Console.WriteLine("Connecting to Vault KV Secrets Engine...");
                // Fetch the secret from the KV Secrets Engine (v2)
                Secret<SecretData> secret = await vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync(
                    path: "configs",
                    mountPoint: "database"
                );
                Console.WriteLine("Successfully connected to Vault and retrieved secret.");

                // Retrieve values from the secret
                Console.WriteLine("Extracting username and password from secret data...");
                dbUsername = secret.Data.Data["username"].ToString();
                dbPassword = secret.Data.Data["password"].ToString();

                Console.WriteLine($"Database Username: {dbUsername}");
                Console.WriteLine("Database Password: [REDACTED FOR SECURITY]");
                Console.WriteLine("Database credentials extracted successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: Failed to fetch secrets from Vault: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Console.WriteLine("Continuing with empty credentials - this may cause database connection issues.");
            }

            // Build the connection string dynamically
            Console.WriteLine("Building database connection string...");
            var connectionString = $"Server=localhost,1433;Database=EventBus;User Id={dbUsername};Password={dbPassword};TrustServerCertificate=true";
            Console.WriteLine("Database connection string built (password redacted for security).");

            // Create and run the web application
            Console.WriteLine("Creating WebApplication builder...");
            var builder = WebApplication.CreateBuilder(args);
            Console.WriteLine("WebApplication builder created successfully.");

            // Add services to the container
            Console.WriteLine("Configuring database services...");
            builder.Services.AddDbContext<GamesContext>(options =>
                options.UseSqlServer(connectionString)
            );
            Console.WriteLine("Database services configured successfully.");

            // Add services for controllers
            Console.WriteLine("Adding controller services...");
            builder.Services.AddControllers();
            Console.WriteLine("Controller services added successfully.");

            // Add services for cache invalidation
            Console.WriteLine("Adding cache invalidation service...");
            builder.Services.AddSingleton<CacheInvalidationService>();
            Console.WriteLine("Cache invalidation service added successfully.");

            Console.WriteLine("Configuring Redis connection...");
            // Add services for Redis
            // Redis Configuration
            var redisConfiguration = new ConfigurationOptions
            {
                EndPoints = { "localhost:6380" }, // Docker service name and port
                AbortOnConnectFail = false,  // Retry if the connection fails initially
                AllowAdmin = true,           // Enable administrative commands if needed
                ConnectRetry = 3,            // Retry attempts
                KeepAlive = 180              // Prevent connection from timing out
            };
            Console.WriteLine("Redis configuration created with endpoint: localhost:6380");

            // Connect to Redis
            Console.WriteLine("Attempting to connect to Redis...");
            try
            {
                var redis = ConnectionMultiplexer.Connect(redisConfiguration);
                builder.Services.AddSingleton<IConnectionMultiplexer>(redis);
                builder.Services.AddSingleton<IDatabase>(sp => sp.GetRequiredService<IConnectionMultiplexer>().GetDatabase());
                Console.WriteLine("Successfully connected to Redis and registered services.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: Failed to connect to Redis: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Console.WriteLine("Application may continue but Redis-dependent features will not work.");
            }

            // Add hosted service for game cleanup
            Console.WriteLine("Adding game cleanup hosted service...");
            builder.Services.AddHostedService<GameCleanupService>();
            Console.WriteLine("Game cleanup service added successfully.");

            // Configure Orleans
            Console.WriteLine("Configuring Orleans silo...");
            builder.Host.UseOrleans(siloBuilder =>
            {
                Console.WriteLine("Setting up localhost clustering for Orleans...");
                // Use localhost clustering for Orleans
                siloBuilder.UseLocalhostClustering();
                Console.WriteLine("Orleans localhost clustering configured.");

                Console.WriteLine("Configuring Orleans Dashboard on port 8080...");
                // Enable Orleans Dashboard and specify port
                siloBuilder.UseDashboard(options =>
                {
                    options.Port = 8080; // Specify the dashboard port
                });
                Console.WriteLine("Orleans Dashboard configured successfully.");
            });
            Console.WriteLine("Orleans configuration completed.");

            // Build the application
            Console.WriteLine("Building the web application...");
            var app = builder.Build();
            Console.WriteLine("Web application built successfully.");

            // Create database
            Console.WriteLine("Ensuring database is created...");
            try
            {
                using (var scope = app.Services.CreateScope())
                {
                    Console.WriteLine("Creating database scope...");
                    var db = scope.ServiceProvider.GetRequiredService<GamesContext>();
                    Console.WriteLine("Database context retrieved. Ensuring database exists...");
                    db.Database.EnsureCreated();
                    Console.WriteLine("Database creation completed successfully.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: Failed to create database: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Console.WriteLine("Application may continue but database-dependent features will not work.");
            }

            // Add middleware to the app pipeline
            Console.WriteLine("Configuring application middleware pipeline...");
            Console.WriteLine("Adding HTTPS redirection middleware...");
            app.UseHttpsRedirection();

            Console.WriteLine("Adding authorization middleware...");
            app.UseAuthorization();

            Console.WriteLine("Adding static files middleware...");
            app.UseStaticFiles();
            Console.WriteLine("Middleware pipeline configured successfully.");

            // Map controllers (endpoints)
            Console.WriteLine("Mapping controller endpoints...");
            app.MapControllers();
            Console.WriteLine("Controller endpoints mapped successfully.");

            // Redirect to index.html on root URL
            Console.WriteLine("Configuring root URL redirect to index.html...");
            app.MapGet("/", () => Results.Redirect("/index.html"));
            Console.WriteLine("Root URL redirect configured successfully.");

            // Run the application
            Console.WriteLine("=== Starting ChessSilo Application ===");
            Console.WriteLine("Application is now ready to serve requests.");
            Console.WriteLine("Orleans Dashboard available at: http://localhost:8080");
            Console.WriteLine("Main application will be available on the configured port.");
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