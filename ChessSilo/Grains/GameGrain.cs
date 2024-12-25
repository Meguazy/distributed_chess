using Orleans;
using System.Threading.Tasks;
using System.Threading;

public class GameGrain : Grain, IGameGrain
{
    private ChessGame _chessGame;

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {   
        // Wait for base activation to complete
        await base.OnActivateAsync(cancellationToken);
    }

    public async Task<bool> StartGameAsync(IPlayerGrain playerWhite, IPlayerGrain playerBlack)
    {
        // Asynchronously get player names
        var whitePlayerName = await playerWhite.GetNameAsync();
        var blackPlayerName = await playerBlack.GetNameAsync();

        // Initialize ChessGame with fetched player names and a new Chessboard
        _chessGame = new ChessGame(
            new ChessGame.Player(whitePlayerName), 
            new ChessGame.Player(blackPlayerName),
            new Chessboard()  // Initialize Chessboard
        );

        _chessGame.StartGame(); // Start the game
        return true;
    }

    public Task<bool> MakeMoveAsync(string move)
    {
        return Task.FromResult(_chessGame.MakeMove(move));
    }

    public async Task<string[,]> GetBoardStateAsync()
    {   
        Chessboard _chessboard = _chessGame.Chessboard;
        return _chessboard.GetBoardState(); // Assuming _chessboard is your Chessboard instance
    }
}
