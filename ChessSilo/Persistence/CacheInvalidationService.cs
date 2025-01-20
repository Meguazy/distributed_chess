using StackExchange.Redis;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;

namespace ChessSilo.Persistence
{
    public class CacheInvalidationService
    {
        private readonly IDatabase _redisDatabase;
        private readonly ILogger<CacheInvalidationService> _logger;

        public CacheInvalidationService(IDatabase redisDatabase, ILogger<CacheInvalidationService> logger)
        {
            _redisDatabase = redisDatabase;
            _logger = logger;
        }

        // Invalidate the cache for a specific game board state
        public async Task InvalidateBoardStateAsync(Guid gameId)
        {
            var cacheKey = $"game:board:{gameId}";
            bool result = await _redisDatabase.KeyDeleteAsync(cacheKey);
            if (result)
            {
                _logger.LogInformation("Cache invalidated for GameId: {GameId}", gameId);
            }
            else
            {
                _logger.LogWarning("No cache found to invalidate for GameId: {GameId}", gameId);
            }
        }
    }
}