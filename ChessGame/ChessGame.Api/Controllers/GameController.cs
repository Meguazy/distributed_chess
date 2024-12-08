// ChessGame.Api/Controllers/GameController.cs
using Microsoft.AspNetCore.Mvc;
using Orleans;
using ChessGame.Grains.Interfaces;

namespace ChessGame.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly IClusterClient _client;

        public GameController(IClusterClient client)
        {
            _client = client;
        }

        // Endpoint to start a new game
        [HttpPost("start")]
        public async Task<IActionResult> StartGame([FromBody] string player1, [FromBody] string player2)
        {
            var gameGrain = _client.GetGrain<IGameGrain>(Guid.NewGuid());
            await gameGrain.StartGame(player1, player2);
            return Ok("Game started!");
        }

        // Endpoint to submit a move
        [HttpPost("{gameId}/move")]
        public async Task<IActionResult> SubmitMove(Guid gameId, [FromBody] string move)
        {
            var gameGrain = _client.GetGrain<IGameGrain>(gameId);
            var result = await gameGrain.SubmitMove("player_id", move); // Replace with real player ID
            return Ok(result);
        }

        // Endpoint to get the current board state
        [HttpGet("{gameId}/state")]
        public async Task<IActionResult> GetBoardState(Guid gameId)
        {
            var gameGrain = _client.GetGrain<IGameGrain>(gameId);
            var state = await gameGrain.GetBoardState();
            return Ok(state);
        }
    }
}
