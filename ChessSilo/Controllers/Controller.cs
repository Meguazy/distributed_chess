using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;  // For logging
using Orleans;
using System.Threading.Tasks;
using System;

namespace ChessSilo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChessGameController : ControllerBase
    {
        private readonly IClusterClient _clusterClient;
        private readonly ILogger<ChessGameController> _logger;  // Inject ILogger

        // Constructor injection for IClusterClient and ILogger
        public ChessGameController(IClusterClient clusterClient, ILogger<ChessGameController> logger)
        {
            _clusterClient = clusterClient;
            _logger = logger;  // Assign logger
        }

        [HttpPost("start")]
        public async Task<IActionResult> StartGame([FromBody] StartGameRequest request)
        {
            // Log starting game request
            _logger.LogInformation("Starting a new game: White - {PlayerWhite}, Black - {PlayerBlack}", 
                request.PlayerWhiteName, request.PlayerBlackName);

            try{
                var gameGrain = _clusterClient.GetGrain<IGameGrain>(Guid.NewGuid());
                var playerWhiteGrain = _clusterClient.GetGrain<IPlayerGrain>(Guid.NewGuid());
                var playerBlackGrain = _clusterClient.GetGrain<IPlayerGrain>(Guid.NewGuid());

                await playerWhiteGrain.SetNameAsync(request.PlayerWhiteName);
                await playerBlackGrain.SetNameAsync(request.PlayerBlackName);

                await gameGrain.StartGameAsync(playerWhiteGrain, playerBlackGrain);

                // Log game started successfully
                _logger.LogInformation("Game started successfully between {PlayerWhite} and {PlayerBlack}.", 
                    request.PlayerWhiteName, request.PlayerBlackName);

                return Ok("Game started.");
            }
            catch (Exception ex){
                // Log any exceptions
                _logger.LogError(ex, "Error occurred while starting the game.");
                return StatusCode(500, "An error occurred while starting the game.");
            }
        }

        [HttpPost("move")]
        public async Task<IActionResult> MakeMove([FromBody] MakeMoveRequest request)
        {
            // Log move request
            _logger.LogInformation("Received move request for game {GameId}: {Move}", 
                request.GameId, request.Move);

            try
            {
                var gameGrain = _clusterClient.GetGrain<IGameGrain>(request.GameId);
                var success = await gameGrain.MakeMoveAsync(request.Move);

                if (success)
                {
                    // Log successful move
                    _logger.LogInformation("Move {Move} made successfully for game {GameId}.", 
                        request.Move, request.GameId);
                    return Ok("Move successful.");
                }
                else
                {
                    // Log invalid move
                    _logger.LogWarning("Invalid move attempted: {Move} for game {GameId}.", 
                        request.Move, request.GameId);
                    return BadRequest("Invalid move.");
                }
            }
            catch (Exception ex)
            {
                // Log any exceptions
                _logger.LogError(ex, "Error occurred while processing the move for game {GameId}.", request.GameId);
                return StatusCode(500, "An error occurred while processing the move.");
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
    }
}