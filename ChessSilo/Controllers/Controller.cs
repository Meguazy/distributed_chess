using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Orleans;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessSilo.Persistence; // Add this line to import the Game class
using Microsoft.EntityFrameworkCore;  // This is required for ToListAsync()
using Newtonsoft.Json;  // Add this line to import JsonConvert
using StackExchange.Redis;  // Add this line to import IDatabase

namespace ChessSilo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChessGameController : ControllerBase
    {
        private readonly IClusterClient _clusterClient;
        private readonly ILogger<ChessGameController> _logger;
        private readonly GamesContext _dbContext;
        private readonly IDatabase _redisDatabase;
        private readonly CacheInvalidationService _cacheInvalidationService;

        public ChessGameController(IClusterClient clusterClient, ILogger<ChessGameController> logger, GamesContext dbContext, CacheInvalidationService cacheInvalidationService, IDatabase redisDatabase)
        {
            _clusterClient = clusterClient;
            _redisDatabase = redisDatabase ?? throw new ArgumentNullException(nameof(redisDatabase));
            _logger = logger;
            _dbContext = dbContext;
            _cacheInvalidationService = cacheInvalidationService;
        }

        [HttpPost("start")]
        public async Task<IActionResult> StartGame([FromBody] StartGameRequest request)
        {
            _logger.LogInformation("Starting a new game: White - {PlayerWhite}, Black - {PlayerBlack}", 
                request.PlayerWhiteName, request.PlayerBlackName);

            await using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var gameId = Guid.NewGuid();

                var game = new Game()
                {
                    GameId = gameId,
                    Description = $"{request.PlayerWhiteName} vs {request.PlayerBlackName}",
                    PlayerBlack = request.PlayerBlackName,
                    PlayerWhite = request.PlayerWhiteName
                };

                _dbContext.Add(game);
                int rowsAffected = await _dbContext.SaveChangesAsync();
                _logger.LogInformation($"Rows affected: {rowsAffected}");

                // Commit the transaction if all operations succeed
                await transaction.CommitAsync();

                _logger.LogInformation("Game started successfully between {PlayerWhite} and {PlayerBlack}. GameId: {GameId}",
                    request.PlayerWhiteName, request.PlayerBlackName, gameId);

                var gameGrain = _clusterClient.GetGrain<IGameGrain>(gameId);
                var playerWhiteGrain = _clusterClient.GetGrain<IPlayerGrain>(Guid.NewGuid());
                var playerBlackGrain = _clusterClient.GetGrain<IPlayerGrain>(Guid.NewGuid());

                await playerWhiteGrain.SetNameAsync(request.PlayerWhiteName);
                await playerBlackGrain.SetNameAsync(request.PlayerBlackName);

                await gameGrain.StartGameAsync(playerWhiteGrain, playerBlackGrain);
                
                return Ok(new { GameId = gameId });
            }
            catch (Exception ex)
            {
                // Rollback if something goes wrong
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error occurred while starting the game.");
                return StatusCode(500, "An error occurred while starting the game.");
            }
        }

        [HttpPost("move")]
        public async Task<IActionResult> MakeMove([FromBody] MakeMoveRequest request)
        {
            _logger.LogInformation("Received move request for game {GameId}: {Move}", request.GameId, request.Move);

            try
            {
                var gameGrain = _clusterClient.GetGrain<IGameGrain>(request.GameId);
                var success = await gameGrain.MakeMoveAsync(request.Move);

                // Handle the move logic here
                var isGameOver = false;  // Replace with actual game over logic

                if (isGameOver)
                {
                    // End the game and update status in the database
                    await EndGameAsync(request.GameId, request.PlayerName); 
                    return Ok("Game over.");
                }

                if (success)
                {
                    _logger.LogInformation("Move {Move} made successfully for game {GameId}.", request.Move, request.GameId);
                    await _cacheInvalidationService.InvalidateBoardStateAsync(request.GameId);
                    return Ok("Move successful.");
                }
                else
                {
                    _logger.LogWarning("Invalid move attempted: {Move} for game {GameId}.", request.Move, request.GameId);
                    return BadRequest("Invalid move.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing the move for game {GameId}.", request.GameId);
                return StatusCode(500, "An error occurred while processing the move.");
            }
        }

        [HttpGet("games")]
        public async Task<IActionResult> GetActiveGames()
        {
            _logger.LogInformation("Fetching active games from the database...");

            try
            {
                // Use DateTime.UtcNow for comparison if the database stores time in UTC
                var activeGames = await _dbContext.Games
                    .Where(game => game.Status == "Active" && game.StartedOn <= DateTime.UtcNow)
                    .Select(game => new 
                    {
                        GameId = game.GameId,
                        Description = game.Description,
                        StartedOn = game.StartedOn
                    })
                    .ToListAsync();

                // Log the active games to verify the structure
                _logger.LogInformation("Active games: " + string.Join(", ", activeGames.Select(game => $"GameId: {game.GameId}, Description: {game.Description}")));

                // Return the result
                return Ok(activeGames);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching active games.");
                return StatusCode(500, "An error occurred while fetching the active games.");
            }
        }

        public async Task EndGameAsync(Guid gameId, string winner)
        {
            var game = await _dbContext.Games.FindAsync(gameId);

            if (game != null)
            {
                game.Status = "Finished"; // or set a 'Winner' property if you want to track the winner
                game.Winner = winner;
                game.EndedOn = DateTime.UtcNow; // Mark the time when the game ended

                await _dbContext.SaveChangesAsync();
            }
        }

        [HttpGet("board")]
        public async Task<IActionResult> GetBoard([FromQuery] string gameId)
        {
            IGameGrain gameGrain;
            Guid gameGuid;
            var boardStateList = new List<List<string>>();

            // Parse the gameId to a GUID
            try
            {
                gameGuid = Guid.Parse(gameId);
                gameGrain = _clusterClient.GetGrain<IGameGrain>(gameGuid);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching game.");
                return StatusCode(500, "The game does not exist.");
            }

            // Check if the board state is cached
            var cacheKey = $"game:board:{gameGuid}";
            int retryCount = 5;
            RedisValue cachedBoardState = RedisValue.Null;
            while (retryCount > 0)
            {
                try
                {
                    cachedBoardState = await _redisDatabase.StringGetAsync(cacheKey);
                    break;
                }
                catch (RedisConnectionException ex)
                {
                    _logger.LogError(ex, "Retrying Redis connection...");
                    retryCount--;
                    await Task.Delay(2000); // Wait 2s before retrying
                }
            }

            if (cachedBoardState.HasValue)
            {
                _logger.LogInformation("Returning cached board state for GameId: {GameId}", gameGuid);
                if (cachedBoardState.HasValue)
                {
                    boardStateList = JsonConvert.DeserializeObject<List<List<string>>>(cachedBoardState!);
                }
                return Ok(boardStateList);
            }
            else
            {
                var boardState = await gameGrain.GetBoardStateAsync();  // Await the async call to get the board state
                _logger.LogInformation("BOARD STATE:\n{BoardState}", boardState);  // Use the parameterized log format to log the boardState

                for (int i = 0; i < boardState.GetLength(0); i++)
                {
                    var row = new List<string>();
                    for (int j = 0; j < boardState.GetLength(1); j++)
                    {
                        row.Add(boardState[i, j]);
                    }
                    boardStateList.Add(row);
                }

                // Cache the board state
                await _redisDatabase.StringSetAsync(cacheKey, JsonConvert.SerializeObject(boardStateList), TimeSpan.FromMinutes(5));

                return Ok(boardStateList);  // Return the list of lists
            }
        }

    }

    public class StartGameRequest
    {
        public required string PlayerWhiteName { get; set; }
        public required string PlayerBlackName { get; set; }
    }

    public class MakeMoveRequest
    {    
        public required Guid GameId { get; set; }
        public required string Move { get; set; }        
        public required string PlayerName { get; set; }
    }
}
