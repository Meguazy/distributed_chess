using Microsoft.AspNetCore.Mvc;
using Orleans;
using System.Threading.Tasks;
using System;

[Route("api/[controller]")]
[ApiController]
public class ChessGameController : ControllerBase
{
    private readonly IClusterClient _clusterClient;

    public ChessGameController(IClusterClient clusterClient)
    {
        _clusterClient = clusterClient;
    }

    [HttpPost("start")]
    public async Task<IActionResult> StartGame([FromBody] StartGameRequest request)
    {
        var gameGrain = _clusterClient.GetGrain<IGameGrain>(Guid.NewGuid());
        var playerWhiteGrain = _clusterClient.GetGrain<IPlayerGrain>(Guid.NewGuid());
        var playerBlackGrain = _clusterClient.GetGrain<IPlayerGrain>(Guid.NewGuid());

        await playerWhiteGrain.SetNameAsync(request.PlayerWhiteName);
        await playerBlackGrain.SetNameAsync(request.PlayerBlackName);

        await gameGrain.StartGameAsync(playerWhiteGrain, playerBlackGrain);

        return Ok("Game started.");
    }

    [HttpPost("move")]
    public async Task<IActionResult> MakeMove([FromBody] MakeMoveRequest request)
    {
        var gameGrain = _clusterClient.GetGrain<IGameGrain>(request.GameId);
        var success = await gameGrain.MakeMoveAsync(request.Move);

        if (success)
            return Ok("Move successful.");
        else
            return BadRequest("Invalid move.");
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
