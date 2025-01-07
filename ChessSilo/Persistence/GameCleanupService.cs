using Microsoft.Extensions.Logging;
using System.Threading;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;
using Microsoft.EntityFrameworkCore;

public class GameCleanupService : IHostedService
{
    private readonly ILogger<GameCleanupService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public GameCleanupService(ILogger<GameCleanupService> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // You can run tasks here on startup if needed
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Cleaning up active games during shutdown...");

        using (var scope = _serviceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<GamesContext>();

            var activeGames = await dbContext.Games
                    .Where(game => game.Status == "Active")
                    .ToListAsync();

            foreach (var game in activeGames)
            {
                // Assuming you end the game if it was left in an active state
                game.Status = "Finished";
                game.EndedOn = DateTime.UtcNow;

                await dbContext.SaveChangesAsync();
            }
            _logger.LogInformation("Cleanup complete. Active games marked as finished.");
        }
    }
}
