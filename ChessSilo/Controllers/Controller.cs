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

namespace ChessSilo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChessGameController : ControllerBase
    {
        private readonly IClusterClient _clusterClient;
        private readonly ILogger<ChessGameController> _logger;
        private readonly GamesContext _dbContext;

        public ChessGameController(IClusterClient clusterClient, ILogger<ChessGameController> logger, GamesContext dbContext)
        {
            _clusterClient = clusterClient;
            _logger = logger;
            _dbContext = dbContext;
        }

        [HttpPost("start")]
        public async Task<IActionResult> StartGame([FromBody] StartGameRequest request)
        {
            _logger.LogInformation("Starting a new game: White - {PlayerWhite}, Black - {PlayerBlack}", 
                request.PlayerWhiteName, request.PlayerBlackName);

            try
            {
                var gameId = Guid.NewGuid();
                var gameGrain = _clusterClient.GetGrain<IGameGrain>(gameId);
                var playerWhiteGrain = _clusterClient.GetGrain<IPlayerGrain>(Guid.NewGuid());
                var playerBlackGrain = _clusterClient.GetGrain<IPlayerGrain>(Guid.NewGuid());

                await playerWhiteGrain.SetNameAsync(request.PlayerWhiteName);
                await playerBlackGrain.SetNameAsync(request.PlayerBlackName);

                await gameGrain.StartGameAsync(playerWhiteGrain, playerBlackGrain);

                var game = new Game()
                {
                    GameId = gameId,
                    Description = $"{request.PlayerWhiteName} vs {request.PlayerBlackName}"
                };

                _dbContext.Add(game);
                int rowsAffected = await _dbContext.SaveChangesAsync();
                _logger.LogInformation($"Rows affected: {rowsAffected}");

                _logger.LogInformation("Game started successfully between {PlayerWhite} and {PlayerBlack}. GameId: {GameId}",
                    request.PlayerWhiteName, request.PlayerBlackName, gameId);

                return Ok(new { GameId = gameId });
            }
            catch (Exception ex)
            {
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

                if (success)
                {
                    _logger.LogInformation("Move {Move} made successfully for game {GameId}.", request.Move, request.GameId);
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
                    .Where(game => game.StartedOn <= DateTime.UtcNow)
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

        [HttpGet("board")]
        public async Task<IActionResult> GetBoard([FromQuery] string gameId)
        {
            IGameGrain gameGrain;
            try
            {
                gameGrain = _clusterClient.GetGrain<IGameGrain>(Guid.Parse(gameId)); // Assuming gameId is a GUID
            }
            catch (Exception ex)
            {
                return StatusCode(500, "The game does not exist.");
            }

            var boardState = await gameGrain.GetBoardStateAsync();  // Await the async call to get the board state
            _logger.LogInformation("BOARD STATE:\n{BoardState}", boardState);  // Use the parameterized log format to log the boardState

            var boardStateList = new List<List<string>>();

            for (int i = 0; i < boardState.GetLength(0); i++)
            {
                var row = new List<string>();
                for (int j = 0; j < boardState.GetLength(1); j++)
                {
                    row.Add(boardState[i, j]);
                }
                boardStateList.Add(row);
            }

            return Ok(boardStateList);  // Return the list of lists
            //return Ok(boardState);
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
                
    }
}
